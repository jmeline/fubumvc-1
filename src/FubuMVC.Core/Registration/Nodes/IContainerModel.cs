using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Nodes
{
    public interface IContainerModel
    {
        /// <summary>
        ///   Generates an ObjectDef object that creates an IoC agnostic
        ///   configuration model of the real Behavior objects for this chain
        /// </summary>
        /// <returns></returns>
        ObjectDef ToObjectDef();
    }
}