using System.Collections;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Rules;
using Habanero.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Habanero.Testability
{
    /// <summary>
    /// The BOTestFactory is a factory used to construct a Business Object for testing.
    /// The Constructed Business object can be constructed a a valid (i.e. saveable Business object)
    /// <see cref="CreateValidBusinessObject"/> .<br/>
    /// A Valid Property Value can also be generated for any particular Prop using one of the overloads of <see cref="GetValidPropValue(IBOProp)"/>,
    /// <see cref="GetValidPropValue(Habanero.Base.IPropDef)"/>, <see cref="GetValidPropValue(Habanero.Base.IBusinessObject,string)"/>
    /// <see cref="GetValidPropValue(System.Type,string)"/>.<br/>
    /// A Valid Relationship can be generated for any particular relationship using <see cref="GetValidRelationshipValue"/>.<br/>
    /// Although all of these are valid methods of using the BOTestFactory you are most likely to use
    /// the Generic BOTestFactory <see cref="BOTestFactory{T}"/> this test factory has even more powerfull
    /// mechanisms to use for generating valid Relationship and PropValues.
    /// </summary>
    public class BOTestFactory
    {
        private readonly Type _boType;
        protected readonly BODefaultValueRegistry _defaultValueRegistry = new BODefaultValueRegistry();
        //If a generator has been registerd for a PropDef then this generator should be used for all instances of BOTestFactory
        // hence the singleton is used. If for a particular test factory different behaviour is required then
        // the _validValueGenRegistry can be set to any valid variable in the constructor.
        protected readonly BOPropValueGeneratorRegistry _validValueGenRegistry = BOPropValueGeneratorRegistry.Instance;

        private const string DEVELOPER_MESSAGE =
            "The BOTestFactory class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.";

        public BOTestFactory(Type boType)
        {
            _boType = boType;
            SetCompulsorySingleRelationships = true;
            this.BOFactory = new BOFactory();
        }

        protected virtual IBusinessObject CreateBusinessObject()
        {
            return this.BOFactory.CreateBusinessObject(_boType);
        }

        //Creates a business object with only its default values set.
        public virtual IBusinessObject CreateDefaultBusinessObject()
        {
            return this.CreateBusinessObject();
        }
        /// <summary>
        /// Creates a business object with all of its compulsory properties and 
        /// Relationships set to Valid values.
        /// </summary>
        /// <returns></returns>
        public virtual IBusinessObject CreateValidBusinessObject()
        {
            IBusinessObject businessObject = this.CreateBusinessObject();
            this.UpdateCompulsoryProperties(businessObject);
            return businessObject;
        }

        /// <summary>
        /// Fixes a Business object that has been created. I.e. if the 
        /// Business object has any <see cref="InterPropRule"/>s then
        /// these rules are used to ensure that the Property values do not
        /// conflict with the InterPropRules.
        /// </summary>
        /// <param name="bo"></param>
        public virtual void FixInvalidInterPropRules(IBusinessObject bo)
        {
            if (bo == null) return;
            if (!bo.Status.IsValid())
            {
                IEnumerable<IBusinessObjectRule> businessObjectRules = GetBusinessObjectRules(bo);
                if (businessObjectRules != null)
                {
                    IEnumerable<string> invalidInterPropRuleForProp =
                        from businessObjectRule in businessObjectRules.OfType<InterPropRule>()
                        where !businessObjectRule.IsValid(bo)
                        select businessObjectRule.RightProp.PropertyName;
                    foreach (string rightPropName in invalidInterPropRuleForProp)
                    {
                        bo.SetPropertyValue(rightPropName, this.GetValidPropValue(bo, rightPropName));
                    }
                }
            }
        }

        private static IEnumerable<IBusinessObjectRule> GetBusinessObjectRules(IBusinessObject bo)
        {
            if (bo == null) return null;
            var privateMethodInfo = ReflectionUtilities.GetPrivateMethodInfo(bo.GetType(), "GetBusinessObjectRules");
            return (privateMethodInfo.Invoke(bo, new object[0]) as IList<IBusinessObjectRule>);
        }

        protected static IPropDef GetPropDef(Type type, string propName, bool raiseErrIfNotExists)
        {
            ValidateClassDef(type);
            IClassDef classDef = ClassDef.ClassDefs[type];

            return GetPropDef(classDef, propName, raiseErrIfNotExists);
        }

        protected static IPropDef GetPropDef(IClassDef classDef, string propName, bool raiseErrIfNotExists)
        {
            IPropDef def = classDef.GetPropDef(propName, false);
            if (raiseErrIfNotExists) ValidateProp(classDef.ClassType, def, propName);
            return def;
        }

        protected static IRelationshipDef GetRelationshipDef(Type type, string relationshipName,
                                                             bool raiseErrIfNotExists)
        {
            ValidateClassDef(type);
            var classDef = ClassDef.ClassDefs[type];
            var relationshipDef = classDef.GetRelationship(relationshipName);
            if (raiseErrIfNotExists) ValidateRelationshipDef(type, relationshipDef, relationshipName);
            return relationshipDef;
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="boProp"/>
        /// using any <see cref="IPropRule"/>s for the Prop.
        /// Note_ this value does take into consideration any 
        /// <see cref="InterPropRule"/>s </summary>
        /// <param name="boProp"></param>
        /// <returns></returns>
        public virtual object GetValidPropValue(IBOProp boProp)
        {
            if (boProp == null) return null;
            IPropDef propDef = boProp.PropDef;
            if (boProp.BusinessObject == null) return this.GetValidPropValue(propDef);
            return this.GetValidPropValue(boProp.BusinessObject, propDef.PropertyName);
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="propDef"/>
        /// using any <see cref="IPropRule"/>s for the Prop.
        /// Note_ this value does not take into consideration any 
        /// <see cref="InterPropRule"/>s
        /// </summary>
        public virtual object GetValidPropValue(IPropDef propDef)
        {
            if (_defaultValueRegistry.IsRegistered(propDef.PropertyName))
            {
                return _defaultValueRegistry.Resolve(propDef.PropertyName);
            }

            ValidValueGenerator generator = GetValidValueGenerator(propDef);
            return ((generator == null) ? null : generator.GenerateValidValue());
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="propName"/>
        /// for the Business object of type <paramref name="classDef"/>
        /// using any <see cref="IPropRule"/>s for the Prop.
        /// Note_ this value does nto atake into consideration any 
        /// <see cref="InterPropRule"/>s
        /// </summary>
        protected virtual object GetValidPropValue(IClassDef classDef, string propName)
        {
            IPropDef def = GetPropDef(classDef, propName, true);
            return this.GetValidPropValue(def);
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="propName"/>
        /// for the Business object
        /// using any <see cref="IPropRule"/>s for the Prop.
        /// Note_ this value does nto atake into consideration any 
        /// <see cref="InterPropRule"/>s
        /// </summary>
        public virtual object GetValidPropValue(string propName)
        {
            IPropDef def = GetPropDef(_boType, propName, true);
            return this.GetValidPropValue(def);
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="propName"/>
        /// for the Business object of type <paramref name="type"/>
        /// using any <see cref="IPropRule"/>s for the Prop.
        /// Note_ this value does nto atake into consideration any 
        /// <see cref="InterPropRule"/>s
        /// </summary>
        public virtual object GetValidPropValue(Type type, string propName)
        {
            IPropDef def = GetPropDef(type, propName, true);
            return this.GetValidPropValue(def);
        }

        /// <summary>
        /// Returns a valid Value for either a single relationship or a property for the <see cref="BusinessObject"/>'s
        /// relationship or prop is identified by the <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object GetValidValue(string name)
        {
            return GetValidValue(_boType, name);
        }

        /// <summary>
        /// Returns a valid prop value for Property or single relationship with the name <paramref name="name"/> for the 
        /// <see cref="IBusinessObject"/>.
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValidValue(IBusinessObject bo, string name)
        {
            IRelationshipDef relationshipDef = GetRelationshipDef(_boType, name, false);
            if (relationshipDef != null) return this.GetValidRelationshipValue(relationshipDef);
            return GetValidPropValue(bo, name);
        }

        /// <summary>
        /// Returns a valid Value for either a single relationship or a property for the <see cref="BusinessObject"/>'s
        /// relationship or prop is identified by the <paramref name="name"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private object GetValidValue(Type type, string name)
        {
            IRelationshipDef relationshipDef = GetRelationshipDef(type, name, false);
            if (relationshipDef != null) return this.GetValidRelationshipValue(relationshipDef);
            IPropDef def = GetPropDef(type, name, false);
            if (def != null) return this.GetValidPropValue(def);
            ThrowPropDoesNotExist(type, name);
            return null;
        }

        private static void ThrowPropDoesNotExist(Type type, string name)
        {
            string message =
                string.Format(
                    "'{0}' for the ClassDef for '{1}' is not defined as either a SingleRelationshipDef or a PropertyDef",
                    name, type);
            throw new HabaneroDeveloperException(message);
        }

        /// <summary>
        /// Returns a valid Relationship for the <see cref="BusinessObject"/>'s
        /// relationship identified by the <paramref name="relationshipName"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        protected IBusinessObject GetValidRelationshipValue(Type type, string relationshipName)
        {
            IRelationshipDef relationshipDef = GetRelationshipDef(type, relationshipName, true);
            return this.GetValidRelationshipValue(relationshipDef);
        }

        /// <summary>
        /// Returns a Valid Relationship Value for the relationship <paramref name="relationshipDef"/>
        /// </summary>
        /// <param name="relationshipDef"></param>
        /// <returns></returns>
        public virtual IBusinessObject GetValidRelationshipValue(IRelationshipDef relationshipDef)
        {
            string relationshipName = relationshipDef.RelationshipName;
            if (_defaultValueRegistry.IsRegistered(relationshipName))
            {
                return _defaultValueRegistry.Resolve(relationshipName) as IBusinessObject;
            }
            Type classType = relationshipDef.RelatedObjectClassDef.ClassType;
            BOTestFactory boTestFactory = BOTestFactoryRegistry.Instance.Resolve(classType);
            return boTestFactory.CreateSavedBusinessObject();
        }

        /// <summary>
        /// Returns a valid prop value for <paramref name="propName"/> for the 
        /// <see cref="IBusinessObject"/> using any <see cref="IPropRule"/>s for the Prop and any 
        /// <see cref="InterPropRule"/>s for the BusinessObject.
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public object GetValidPropValue(IBusinessObject bo, string propName)
        {
            //TODO brett 18 Mar 2010: This will not take into consideration multiple InterPropRules
            // this should find the most restrictive rule and then return a result using the most restrictive rule.
            IEnumerable<IBusinessObjectRule> businessObjectRules = GetBusinessObjectRules(bo);
            if (businessObjectRules != null)
            {
                IValidValueGeneratorNumeric validValueGenerator;
                IEnumerable<InterPropRule> interPropRules = from rule in businessObjectRules.OfType<InterPropRule>()
                                                            where rule.RightProp.PropertyName == propName
                                                            select rule;
                foreach (InterPropRule businessObjectRule in interPropRules)
                {
                    object leftPropValue = bo.GetPropertyValue(businessObjectRule.LeftProp.PropertyName);
                    if (leftPropValue != null)
                    {
                        validValueGenerator =
                            GetValidValueGenerator(businessObjectRule.RightProp) as IValidValueGeneratorNumeric;
                        if (validValueGenerator != null)
                        {
                            switch (businessObjectRule.ComparisonOp)
                            {
                                case ComparisonOperator.GreaterThan:
                                case ComparisonOperator.GreaterThanOrEqual:
                                    return validValueGenerator.GenerateValidValueLessThan(leftPropValue);

                                case ComparisonOperator.EqualTo:
                                    return leftPropValue;

                                case ComparisonOperator.LessThanOrEqual:
                                case ComparisonOperator.LessThan:
                                    return validValueGenerator.GenerateValidValueGreaterThan(leftPropValue);
                            }
                            return null;
                        }
                    }
                }
                interPropRules = from rule in businessObjectRules.OfType<InterPropRule>()
                                 where rule.LeftProp.PropertyName == propName
                                 select rule;
                foreach (InterPropRule businessObjectRule in interPropRules)
                {
                    object rightPropValue = bo.GetPropertyValue(businessObjectRule.RightProp.PropertyName);
                    if (rightPropValue != null)
                    {
                        validValueGenerator =
                            GetValidValueGenerator(businessObjectRule.RightProp) as IValidValueGeneratorNumeric;
                        if (validValueGenerator != null)
                        {
                            switch (businessObjectRule.ComparisonOp)
                            {
                                case ComparisonOperator.GreaterThan:
                                case ComparisonOperator.GreaterThanOrEqual:
                                    return validValueGenerator.GenerateValidValueGreaterThan(rightPropValue);

                                case ComparisonOperator.EqualTo:
                                    return rightPropValue;

                                case ComparisonOperator.LessThanOrEqual:
                                case ComparisonOperator.LessThan:
                                    return validValueGenerator.GenerateValidValueLessThan(rightPropValue);
                            }
                            return null;
                        }
                    }
                }
            }
            return this.GetValidPropValue(bo.ClassDef, propName);
        }

        /// <summary>
        /// returns a valid value generator for of the specified type based on the 
        /// <see cref="IPropDef"/>.<see cref="IPropDef.PropertyType"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <returns></returns>
        public ValidValueGenerator GetValidValueGenerator(IPropDef propDef)
        {
            if (propDef.HasLookupList())
            {
                return new ValidValueGeneratorLookupList(propDef);
            }
            if (propDef.PropertyType.IsEnum)
            {
                return new ValidValueGeneratorEnum(propDef);
            }

            return _validValueGenRegistry.Resolve(propDef);
        }

        /// <summary>
        /// Sets the value of the <see cref="IBOProp"/> to a valid value.
        /// This is primarily used internally.
        /// </summary>
        /// <param name="boProp"></param>
        public virtual void SetPropValueToValidValue(IBOProp boProp)
        {
            if (boProp == null) return;
            if (boProp.Value != null
                && !_defaultValueRegistry.IsRegistered(boProp.PropertyName)
                && !_validValueGenRegistry.IsRegistered(boProp.PropDef)) return;
            object generateValidValue = this.GetValidPropValue(boProp);
            boProp.Value = generateValidValue;
        }

        /// <summary>
        /// Sets the Value of the <see cref="IMultipleRelationship"/> to a list of values as configured using the WithMany.
        /// </summary>
        /// <param name="multipleRelationship"></param>
        private void SetRelationshipToValidValue(IMultipleRelationship multipleRelationship)
        {
            if (!_defaultValueRegistry.IsRegistered(multipleRelationship.RelationshipName)) return;
            IList lists = (IList) _defaultValueRegistry.Resolve(multipleRelationship.RelationshipName);
            if (lists == null) return;
            foreach (var item in lists)
            {
                multipleRelationship.BusinessObjectCollection.Add(item);
            }
        }

        /// <summary>
        /// Sets the Value of the <see cref="ISingleRelationship"/> to a valid value.
        /// </summary>
        /// <param name="singleRelationship"></param>
        public virtual void SetRelationshipToValidValue(ISingleRelationship singleRelationship)
        {
            if (singleRelationship.GetRelatedObject() != null &&
                !_defaultValueRegistry.IsRegistered(singleRelationship.RelationshipName)) return;
            IBusinessObject validBusinessObject = this.GetValidRelationshipValue(singleRelationship.RelationshipDef);
            singleRelationship.SetRelatedObject(validBusinessObject);
        }

        /// <summary>
        /// Updates any compulsory relationships or properties for
        /// </summary>
        /// <param name="businessObject"></param>
        public virtual void UpdateCompulsoryProperties(IBusinessObject businessObject)
        {
            if (businessObject == null) return;
            UpdatePrimaryKeyProps(businessObject);
            UpdateSingleRelationships(businessObject);
            UpdatedManyRelationships(businessObject);
            UpdateProperties(businessObject);
        }

        private void UpdatePrimaryKeyProps(IBusinessObject businessObject)
        {
            IEnumerable<IBOProp> props = from boProp in businessObject.ID.GetBOPropCol()
                                         where (boProp.PropDef != null)
                                         select boProp;
            foreach (IBOProp boProp in props)
            {
/*                if (boProp.PropDef.Compulsory
                    || _defaultValueRegistry.IsRegistered(boProp.PropertyName)
                    || _validValueGenRegistry.IsRegistered(boProp.PropDef))
                {*/
                this.SetPropValueToValidValue(boProp);
                // }
            }
        }

        private void UpdatedManyRelationships(IBusinessObject businessObject)
        {
            IEnumerable<IMultipleRelationship> multipleRelationships =
                from relationship in businessObject.Relationships.OfType<IMultipleRelationship>()
                where (relationship.RelationshipDef != null)
                select relationship;
            foreach (IMultipleRelationship multipleRel in multipleRelationships)
            {
                if (_defaultValueRegistry.IsRegistered(multipleRel.RelationshipName))
                {
                    this.SetRelationshipToValidValue(multipleRel);
                }
            }
        }

        private void UpdateProperties(IBusinessObject businessObject)
        {
            IEnumerable<IBOProp> props = from boProp in businessObject.Props
                                         where (boProp.PropDef != null)
                                         select boProp;
            foreach (IBOProp boProp in props)
            {
                if (boProp.PropDef.Compulsory
                    || _defaultValueRegistry.IsRegistered(boProp.PropertyName)
                    || _validValueGenRegistry.IsRegistered(boProp.PropDef))
                {
                    this.SetPropValueToValidValue(boProp);
                }
            }
        }

        private void UpdateSingleRelationships(IBusinessObject businessObject)
        {
            IEnumerable<ISingleRelationship> singleRelationships =
                from relationship in businessObject.Relationships.OfType<ISingleRelationship>()
                where (relationship.RelationshipDef != null)
                select relationship;
            foreach (ISingleRelationship singleRelationship in singleRelationships)
            {
                if (MustCreateSinglePropValue(singleRelationship))
                {
                    this.SetRelationshipToValidValue(singleRelationship);
                }
            }
        }

        private bool MustCreateSinglePropValue(ISingleRelationship singleRelationship)
        {
            return (singleRelationship.RelationshipDef.IsCompulsory && SetCompulsorySingleRelationships)
                   || _defaultValueRegistry.IsRegistered(singleRelationship.RelationshipName);
        }

        /// <summary>
        /// Sets the <paramref name="propertyValue"/> for the method Idenfied by the <paramref name="propertyName"/>.
        /// This ensures that when the <see cref="CreateValidBusinessObject"/> or <see cref="BOTestFactory.UpdateCompulsoryProperties"/>
        /// or <see cref="GetValidPropValue(Habanero.Base.IBOProp)"/> and the <see cref="GetValidRelationshipValue"/>
        /// for this Property this value is always used
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetValueFor(string propertyName, object propertyValue)
        {
            _defaultValueRegistry.Register(propertyName, propertyValue);
        }

        protected static void ValidateClassDef(Type type)
        {
            if (!ClassDef.ClassDefs.Contains(type))
            {
                throw new HabaneroDeveloperException(
                    string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), DEVELOPER_MESSAGE);
            }
        }

        private static void ValidateProp(Type type, IPropDef def, string propName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(
                    string.Format("The property '{0}' for the ClassDef for '{1}' is not defined", propName, type),
                    DEVELOPER_MESSAGE);
            }
        }

        protected static void ValidateRelationshipDef(Type type, IRelationshipDef def, string relationshipName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(
                    string.Format("The relationship '{0}' for the ClassDef for '{1}' is not defined", relationshipName,
                                  type), DEVELOPER_MESSAGE);
            }
        }

        protected BOFactory BOFactory { get; private set; }
        protected bool SetCompulsorySingleRelationships { get; set; }

        /// <summary>
        /// Returns a saved valid business object of Type
        /// </summary>
        /// <returns></returns>
        public virtual IBusinessObject CreateSavedBusinessObject()
        {
            IBusinessObject bo = this.CreateBusinessObject();
            this.UpdateCompulsoryProperties(bo);
            bo.Save();
            return bo;
        }
    }
}