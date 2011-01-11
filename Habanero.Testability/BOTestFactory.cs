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
    /// <see cref="CreateValidBusinessObject"/> a Default Busienss object <see cref="CreateDefaultBusinessObject"/>.<br/>
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
        public BOTestFactory()
        {
            this.BOFactory = new BOFactory();
        }

        private IBusinessObject CreateBusinessObject(Type type)
        {
            return this.BOFactory.CreateBusinessObject(type);
        }
        /// <summary>
        /// Creates a business object with its default values set.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual IBusinessObject CreateDefaultBusinessObject(Type type)
        {
            return this.CreateBusinessObject(type);
        }
        /// <summary>
        /// Creates a business object with all of its compulsory properties and 
        /// Relationships set to Valid values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T CreateValidBusinessObject<T>() where T: IBusinessObject
        {
            return (T) this.CreateValidBusinessObject(typeof(T));
        }
        /// <summary>
        /// Creates a business object with all of its compulsory properties and 
        /// Relationships set to Valid values.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual IBusinessObject CreateValidBusinessObject(Type type)
        {
            IBusinessObject businessObject = this.CreateBusinessObject(type);
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
            if (!bo.Status.IsValid())
            {
                IEnumerable<IBusinessObjectRule> businessObjectRules = GetBusinessObjectRules(bo);
                if (businessObjectRules != null)
                {
                    IEnumerable<string> invalidInterPropRuleForProp = from businessObjectRule in businessObjectRules.OfType<InterPropRule>()
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
            if(bo == null) return null;
            var privateMethodInfo = ReflectionUtilities.GetPrivateMethodInfo(bo.GetType(), "GetBusinessObjectRules");
            return (privateMethodInfo.Invoke(bo, new object[0]) as IList<IBusinessObjectRule>);
        }

        private static IPropDef GetPropDef(Type type, string propName)
        {
            ValidateClassDef(type);
            IPropDef def = ClassDef.ClassDefs
                    .First(classDef => (classDef.ClassType == type))
                    .PropDefcol
                    .FirstOrDefault(propDef => propDef.PropertyName == propName);
            ValidateProp(type, def, propName);
            return def;
        }

        protected static IRelationshipDef GetRelationshipDef(Type type, string relationshipName)
        {
            ValidateClassDef(type);
            IRelationshipDef def = ClassDef.ClassDefs
                    .First(classDef => (classDef.ClassType == type))
                    .RelationshipDefCol
                    .FirstOrDefault(relDef => relDef.RelationshipName == relationshipName);
            ValidateRelationshipDef(type, def, relationshipName);
            return def;
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
            ValidValueGenerator generator = GetValidValueGenerator(propDef);
            return ((generator == null) ? null : generator.GenerateValidValue());
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
            IPropDef def = GetPropDef(type, propName);
            return this.GetValidPropValue(def);
        }
        /// <summary>
        /// Returns a Valid Relationship Value for the relationship <paramref name="relationshipDef"/>
        /// </summary>
        /// <param name="relationshipDef"></param>
        /// <returns></returns>
        public virtual IBusinessObject GetValidRelationshipValue(IRelationshipDef relationshipDef)
        {
            Type classType = relationshipDef.RelatedObjectClassDef.ClassType;
            return BOTestFactoryRegistry.Registry.Resolve(classType).CreateValidBusinessObject(classType);
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
                        validValueGenerator = GetValidValueGenerator(businessObjectRule.RightProp) as IValidValueGeneratorNumeric;
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
                        validValueGenerator = GetValidValueGenerator(businessObjectRule.RightProp) as IValidValueGeneratorNumeric;
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
            return this.GetValidPropValue(bo.ClassDef.ClassType, propName);
        }
        /// <summary>
        /// returns a valid value generator for of the specified type based on the 
        /// <see cref="IPropDef"/>.<see cref="IPropDef.PropertyType"/>
        /// </summary>
        /// <param name="propDef"></param>
        /// <returns></returns>
        public static ValidValueGenerator GetValidValueGenerator(IPropDef propDef)
        {
            ValidValueGenerator generator = null;
            if (propDef.HasLookupList())
            {
                return new ValidValueGeneratorLookupList(propDef);
            }
            if (propDef.PropertyType == typeof(string))
            {
                return new ValidValueGeneratorString(propDef);
            }
            if (propDef.PropertyType == typeof(Guid))
            {
                return new ValidValueGeneratorGuid(propDef);
            }
            if (propDef.PropertyType == typeof(int))
            {
                return new ValidValueGeneratorInt(propDef);
            }
            if (propDef.PropertyType == typeof(bool))
            {
                return new ValidValueGeneratorBool(propDef);
            }
            if (propDef.PropertyType == typeof(decimal))
            {
                return new ValidValueGeneratorDecimal(propDef);
            }
            if (propDef.PropertyType == typeof(DateTime))
            {
                return new ValidValueGeneratorDate(propDef);
            }
            if (propDef.PropertyType == typeof(double))
            {
                return new ValidValueGeneratorDouble(propDef);
            }
            if (propDef.PropertyType.IsEnum)
            {
                generator = new ValidValueGeneratorEnum(propDef);
            }
            return generator;
        }
        /// <summary>
        /// Sets the value of the <see cref="IBOProp"/> to a valid value.
        /// This is primarily used internally.
        /// </summary>
        /// <param name="boProp"></param>
        public virtual void SetPropValueToValidValue(IBOProp boProp)
        {
            if (boProp.Value != null) return;
            object generateValidValue = this.GetValidPropValue(boProp);
            boProp.Value = generateValidValue;
        }
        /// <summary>
        /// Sets the Value of the <see cref="ISingleRelationship"/> to a valid value.
        /// </summary>
        /// <param name="compusoryRelationship"></param>
        public virtual void SetRelationshipToValidValue(ISingleRelationship compusoryRelationship)
        {
            if (compusoryRelationship.GetRelatedObject() != null) return;
            IBusinessObject validBusinessObject = this.GetValidRelationshipValue(compusoryRelationship.RelationshipDef);
            compusoryRelationship.SetRelatedObject(validBusinessObject);
        }
        /// <summary>
        /// Updates any compulsory relationships or properties for
        /// </summary>
        /// <param name="businessObject"></param>
        public virtual void UpdateCompulsoryProperties(IBusinessObject businessObject)
        {
            IEnumerable<ISingleRelationship> compulsorySingleRelationship = 
                from relationship in businessObject.Relationships.OfType<ISingleRelationship>()
                where (relationship.RelationshipDef != null) && relationship.RelationshipDef.IsCompulsory
                select relationship;
            foreach (ISingleRelationship compusoryRelationship in compulsorySingleRelationship)
            {
                this.SetRelationshipToValidValue(compusoryRelationship);
            }
            IEnumerable<IBOProp> compulsoryProps = from boProp in businessObject.Props
                where (boProp.PropDef != null) && boProp.PropDef.Compulsory
                select boProp;
            foreach (IBOProp boProp in compulsoryProps)
            {
                this.SetPropValueToValidValue(boProp);
            }
        }

        protected static void ValidateClassDef(Type type)
        {
            if (!ClassDef.ClassDefs.Contains(type))
            {
                throw new HabaneroDeveloperException(string.Format("The ClassDef for '{0}' does not have any classDefs Loaded", type), "This class is designed ot be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.");
            }
        }

        private static void ValidateProp(Type type, IPropDef def, string propName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(string.Format("The property '{0}' for the ClassDef for '{1}' is not defined", propName, type), "This class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.");
            }
        }

        protected static void ValidateRelationshipDef(Type type, IRelationshipDef def, string relationshipName)
        {
            if (def == null)
            {
                throw new HabaneroDeveloperException(string.Format("The relationship '{0}' for the ClassDef for '{1}' is not defined", relationshipName, type), "This class is designed to be used in Testing so it is likely that your classdefs are not being loaded as part of your testing process.");
            }
        }

        protected BOFactory BOFactory { get; private set; }
    }
}

