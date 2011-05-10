using Habanero.Base;

namespace Habanero.Testability.CF
{
    public class TestFixture
    {
        private BOTestFactoryRegistry testFactoryRegistry = BOTestFactoryRegistry.Instance;
        public T CreateObject<T>() where T: class, IBusinessObject
        {
            return testFactoryRegistry.Resolve<T>().CreateValidBusinessObject();
        }

        public void Register<T>(T bo) where T : class, IBusinessObject
        {
            var instanceReturningBOTestFactory = new InstanceReturningBOTestFactory<T>(bo);
            testFactoryRegistry.Register<T>(instanceReturningBOTestFactory);
        }
    }
    public class InstanceReturningBOTestFactory<T>:BOTestFactory<T> where T : class, IBusinessObject
    {
        private readonly T _boToAlwaysReturn;

        public InstanceReturningBOTestFactory(T  boToAlwaysReturn)
        {
            _boToAlwaysReturn = boToAlwaysReturn;
        }
        protected override IBusinessObject CreateBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override T CreateValidBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override void UpdateCompulsoryProperties(IBusinessObject businessObject)
        {
           //Do nothing you are returning an instance as is
        }

        public override T CreateSavedBusinessObject()
        {
            return _boToAlwaysReturn;
        }

        public override T CreateDefaultBusinessObject()
        {
            return _boToAlwaysReturn;
        }
    }
}
