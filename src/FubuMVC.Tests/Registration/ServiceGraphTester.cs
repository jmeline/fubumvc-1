using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuCore;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class ServiceGraphTester
    {
        private ServiceGraph theGraph;
        private ITracedModel theTracedNode;

        [SetUp]
        public void SetUp()
        {
            theGraph = new ServiceGraph();

            theTracedNode = theGraph.As<ITracedModel>();
        }

        [Test]
        public void adding_a_service_registers_a_service_added_event()
        {
            theGraph.Add(typeof(IFoo), new ObjectDef(typeof(Foo)));

            var added = theTracedNode.StagedEvents.Last().ShouldBeOfType<ServiceAdded>();
            added.ServiceType.ShouldEqual(typeof (IFoo));
            added.Def.Type.ShouldEqual(typeof (Foo));

        }

        [Test]
        public void clear_registers_service_removed_events()
        {
            theGraph.Add(typeof(IFoo), new ObjectDef(typeof(Foo)));
            theGraph.Add(typeof(IFoo), new ObjectDef(typeof(Foo2)));
            theGraph.Add(typeof(IFoo), new ObjectDef(typeof(Foo3)));
            theGraph.Add(typeof(IFoo), new ObjectDef(typeof(Foo4)));
        
            theGraph.Clear(typeof(IFoo));

            theTracedNode.StagedEvents.OfType<ServiceRemoved>().Where(x => x.ServiceType == typeof(IFoo))
                .Select(x => x.Def.Type)
                .ShouldHaveTheSameElementsAs(typeof(Foo), typeof(Foo2), typeof(Foo3), typeof(Foo4));

        }

        [Test]
        public void has_any()
        {
            theGraph.HasAny(typeof(IFoo)).ShouldBeFalse();

            theGraph.Add(typeof(IFoo), ObjectDef.ForType<Foo>());

            theGraph.HasAny(typeof(IFoo)).ShouldBeTrue();
        }

        [Test]
        public void fill_type()
        {
            theGraph.Add(typeof(IFoo), ObjectDef.ForType<Foo>());

            theGraph.FillType(typeof(IFoo), typeof(Foo));
            theGraph.FillType(typeof(IFoo), typeof(Foo));
            theGraph.FillType(typeof(IFoo), typeof(Foo));
            theGraph.FillType(typeof(IFoo), typeof(Foo));
            theGraph.ServicesFor(typeof (IFoo)).Count().ShouldEqual(1);

            theGraph.FillType(typeof(IFoo), typeof(Foo2));
            theGraph.ServicesFor(typeof (IFoo)).Select(x => x.Type)
                .ShouldHaveTheSameElementsAs(typeof(Foo), typeof(Foo2));
        }

        public interface IFoo{}
        public class Foo : IFoo{}
        public class Foo2 : IFoo{}
        public class Foo3 : IFoo{}
        public class Foo4 : IFoo{}
    }
}