using Henry.AI.Core.Domain.CodeUnderstanding.MappedEntities;
using Henry.AI.Core.Domain.CodeUnderstanding.UnmappedEntities;
using Henry.AI.Core.Host.CodeUnderstanding.Contracts;
using Henry.AI.Core.Host.CodeUnderstanding.Dtos;
using Henry.AI.Core.Host.CodeUnderstanding.Repositories;
using Rebus.Bus;
using Rebus.Handlers;

namespace Henry.AI.Core.Host.CodeUnderstanding.Handlers;

public class CodeUnderstandingResponseHandler : IHandleMessages<CodeUnderstandingResponse>
{
    private readonly IBus _bus;
    private readonly IClassRepository _classRepository;
    private readonly IConstructorRepository _constructorRepository;
    private readonly IMethodRepository _methodRepository;
    private readonly IPropertyRepository _propertyRepository;

    public CodeUnderstandingResponseHandler(IBus bus, IClassRepository classRepository, IConstructorRepository constructorRepository, IMethodRepository methodRepository, IPropertyRepository propertyRepository)
    {
        _bus = bus;
        _classRepository = classRepository;
        _constructorRepository = constructorRepository;
        _methodRepository = methodRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task Handle(CodeUnderstandingResponse message)
    {
        if (!message.Ok)
        {
            return;
        }
        var classes = message.Classes.Select(c => c.ToClass(message.Namespace)).ToList();
        
        var constructorMapping = message.Classes.ToDictionary(c => c, c => c.Constructors);
        var methodMapping = message.Classes.ToDictionary(c => c, c => c.Methods);
        var propertyMapping = message.Classes.ToDictionary(c => c, c => c.Properties);

        var heritageMapping = message.Classes.ToDictionary(c => c, c => c.Heritages);
        var implementationMapping = message.Classes.ToDictionary(c => c, c => c.Implementations);
        var dependencyMapping = message.Classes.ToDictionary(c => c, c => c.Dependencies);
        
        var createdClassNodes = await HandleClasses(classes);
        var createdConstructorNodes = await HandleConstructors(constructorMapping,message.Namespace);
        var createdMethodNodes = await HandleMethods(methodMapping,message.Namespace);
        var createdPropertyNodes = await HandleProperties(propertyMapping,message.Namespace);
        
        await HandleHerigates(heritageMapping,message.Namespace);
        await HandleImplementations(implementationMapping,message.Namespace);
        await HandleDependencies(dependencyMapping,message.Namespace);
        
        await HandleHasConstructor(createdConstructorNodes,createdClassNodes);
        await HandleHasMethod(createdMethodNodes,createdClassNodes);
        await HandleHasProperty(createdPropertyNodes,createdClassNodes);
    }
    
    private async Task HandleHasProperty(ILookup<string, PropertyNode> propertyLookup,Dictionary<string,ClassNode> classes)
    {
        foreach (var propertyGroup in propertyLookup)
        {
            var className = propertyGroup.Key;
            var propertyNodes = propertyGroup.ToList();
            var classNode = classes[className];
            foreach (var propertyNode in propertyNodes)
            {
                await _classRepository.CreateHasProperty(classNode, propertyNode);
            }
        }
    }
    
    private async Task HandleHasMethod(ILookup<string, MethodNode> methodLookup,Dictionary<string,ClassNode> classes)
    {
        foreach (var methodGroup in methodLookup)
        {
            var className = methodGroup.Key;
            var methodNodes = methodGroup.ToList();
            var classNode = classes[className];
            foreach (var methodNode in methodNodes)
            {
                await _classRepository.CreateHasMethod(classNode, methodNode);
            }
        }
    }


    private async Task HandleHasConstructor(ILookup<string, ConstructorNode> constructorLookup,Dictionary<string,ClassNode> classes)
    {
        foreach (var constructorGroup in constructorLookup)
        {
            var className = constructorGroup.Key;
            var constructorNodes = constructorGroup.ToList();
            var classNode = classes[className];
            foreach (var constructorNode in constructorNodes)
            {
                await _classRepository.CreateConstructedBy(classNode, constructorNode);
            }
        }
    }
    
    private async Task HandleHerigates(Dictionary<TClass, List<THeritage>> heritageMappings, string namespc)
    {
        foreach (var heritageMapping in heritageMappings)
        {
            var tClass = heritageMapping.Key;
            var heritages = heritageMapping.Value;
            foreach (var heritage in heritages)
            {
                var heritageNode = new ClassNode();
                heritageNode.Name = heritage.Name;
                var nodeInDb = await _classRepository.FindByNameOnly(heritageNode);
                if (nodeInDb == null)
                { 
                    await _classRepository.CreateEmpty(heritageNode!);
                }

                var mainClass = await _classRepository.Find(tClass.ToClass(namespc));
                heritageNode = await _classRepository.FindByNameOnly(heritageNode);

                await _classRepository.CreateInheritance(mainClass!, heritageNode!);
            }
        }
    }
    
    
    private async Task HandleImplementations(Dictionary<TClass, List<TImplementation>> implementationMappings, string namespc)
    {
        foreach (var implementationMapping in implementationMappings)
        {
            var tClass = implementationMapping.Key;
            var implementations = implementationMapping.Value;
            foreach (var implementation in implementations)
            {
                var implementationNode = new ClassNode();
                implementationNode.Name = implementation.Name;
                var nodeInDb = await _classRepository.FindByNameOnly(implementationNode);
                if (nodeInDb == null)
                {
                    await _classRepository.CreateEmpty(implementationNode!);
                }

                var mainClass = await _classRepository.Find(tClass.ToClass(namespc));
                implementationNode = await _classRepository.FindByNameOnly(implementationNode);

                await _classRepository.CreateImplementation(mainClass!, implementationNode!);
            }
        }
    }
    
    private async Task HandleDependencies(Dictionary<TClass, List<TDependency>> dependencyMappings, string namespc)
    {
        foreach (var dependencyMapping in dependencyMappings)
        {
            var tClass = dependencyMapping.Key;
            var dependencies = dependencyMapping.Value;
            foreach (var dependency in dependencies)
            {
                var dependencyNode = new ClassNode();
                dependencyNode.Name = dependency.Name;
                var nodeInDb = await _classRepository.FindByNameOnly(dependencyNode);
                if (nodeInDb == null)
                {
                    await _classRepository.CreateEmpty(dependencyNode!);
                }

                var mainClass = await _classRepository.Find(tClass.ToClass(namespc));
                dependencyNode = await _classRepository.FindByNameOnly(dependencyNode);

                var dependencyProperty = new DependencyProperties()
                {
                    Accessibility = dependency.Accessibility,
                    Injected = dependency.Injected
                };
                await _classRepository.CreateDependency(mainClass!, dependencyNode!,dependencyProperty);
            }
        }
    }


    private async Task<Dictionary<string,ClassNode>> HandleClasses(List<ClassNode> classes)
    {
        var createdClassNodes = new List<ClassNode>();
        foreach (var classFromList in classes)
        {
            var existantClass = await _classRepository.FindByNameOnly(classFromList);
            if (existantClass == null)
            {
                await _classRepository.Create(classFromList);
                createdClassNodes.Add(classFromList);
            }
            else
            {
                existantClass.Update(classFromList);
                await _classRepository.Update(existantClass);
                createdClassNodes.Add(existantClass);
            }
        }
        return createdClassNodes.ToDictionary(c => c.Name, c => c);
    }

    private async Task<ILookup<string,ConstructorNode>> HandleConstructors(Dictionary<TClass,List<TConstructor>> constructorsMapping,string namespc)
    {
        var createdConstructorNodes = new List<ConstructorNode>();
        foreach (var constructorMapping in constructorsMapping)
        {
            var tClass = constructorMapping.Key;
            var constructors = constructorMapping.Value;

            foreach (var constructor in constructors)
            {
                var constructorNode = constructor.ToConstructor(tClass.Name,namespc);
                var existantConstructor = await _constructorRepository.Find(constructorNode);
                if (existantConstructor != null)
                {
                    await _constructorRepository.Delete(existantConstructor);
                }
                await _constructorRepository.Create(constructorNode);
                createdConstructorNodes.Add(constructorNode);
            }
        }
        return createdConstructorNodes.ToLookup(c => c.ClassName,c => c);
    }

    private async Task<ILookup<string,MethodNode>> HandleMethods(Dictionary<TClass, List<TMethod>> methodsMapping,string namespc)
    {
        var createdMethodNodes = new List<MethodNode>();
        foreach (var methodMapping in methodsMapping)
        {
            var tClass = methodMapping.Key;
            var methods = methodMapping.Value;
            foreach (var method in methods)
            {
                var methodNode = method.ToMethod(tClass.Name,namespc);
                var existantMethod = await _methodRepository.Find(methodNode);
                if (existantMethod != null)
                {
                    await _methodRepository.Delete(methodNode);
                }
                await _methodRepository.Create(methodNode);
                createdMethodNodes.Add(methodNode);
            }
        }

        return createdMethodNodes.ToLookup(c => c.ClassName,c => c);;
    }

    private async Task<ILookup<string,PropertyNode>> HandleProperties(Dictionary<TClass, List<TProperty>> propertiesMapping,string namespc)
    {
        var createdPropertiesNodes = new List<PropertyNode>();
        foreach (var propertyMapping in propertiesMapping)
        {
            var tClass = propertyMapping.Key;
            var properties = propertyMapping.Value;
            foreach (var property in properties)
            {
                var propertyNode = property.ToProperty(tClass.Name,namespc);
                var existantProperty = await _propertyRepository.Find(propertyNode);
                if (existantProperty != null)
                {
                    await _propertyRepository.Delete(propertyNode);
                }
                await _propertyRepository.Create(propertyNode);
                createdPropertiesNodes.Add(propertyNode);
            }
        }
        return createdPropertiesNodes.ToLookup(c => c.ClassName,c => c);
    }
}