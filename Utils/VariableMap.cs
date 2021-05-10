using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.UnityMonstackCore.Extensions.Collections;
using Plugins.UnityMonstackCore.Loggers;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class VariableMap : IEnumerable<KeyValuePair<string, object>>
    {
        [Serializable]
        public class VariableCondition
        {
            public string name;
            public object value;

            public bool Validate(Dictionary<string, object> variables)
            {
                var currentValue = variables.GetValueOrDefault(name, default);
                return Equals(currentValue, value);
            }
        }

        public delegate void OnUpdatedDelegate();

        public event OnUpdatedDelegate onUpdated;

        protected Dictionary<string, object> variables = new Dictionary<string, object>();

        public VariableMap()
        {
        }

        public VariableMap(Dictionary<string, object> variables)
        {
            foreach (var keyValuePair in variables)
                this.variables[keyValuePair.Key] = keyValuePair.Value;
        }

        public VariableMap SetValue(string key, object value)
        {
            if (variables.TryGetValue(key, out var currentValue) && Equals(currentValue, value))
                return this;

            variables[key] = value;
            TriggerUpdate();
            return this;
        }

        public VariableMap SetOrModifyValue<T>(string key, T value, Func<T, T, T> modifyLambda) where T : class
        {
            if (variables.ContainsKey(key))
                variables[key] = modifyLambda.Invoke(variables[key] as T, value);
            else
                variables[key] = value;

            TriggerUpdate();
            return this;
        }

        public object GetValue(string key)
        {
            return variables[key];
        }

        public T GetValue<T>(string key)
        {
            try
            {
                return (T) variables[key];
            }
            catch (InvalidCastException e)
            {
                UnityLogger.Error($"Failed to get state variable as {typeof(T).Name}, whereas it's {variables[key].GetType().Name}", e);
                throw;
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (variables.TryGetValue(key, out var currentValue) && currentValue is T castedValue)
            {
                value = castedValue;
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKeyAndValue(string key, object value)
        {
            if (!variables.TryGetValue(key, out var currentValue))
                return false;

            return Equals(variables[key], value);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            try
            {
                return (T) variables.GetValueOrDefault(key, defaultValue);
            }
            catch (InvalidCastException e)
            {
                UnityLogger.Error($"Failed to get state variable as {typeof(T).Name}, whereas it's {variables[key].GetType().Name}", e);
                throw;
            }
        }

        public bool Matches(VariableCondition[] conditions)
        {
            if (conditions == null)
                return true;

            foreach (var condition in conditions)
            {
                if (!condition.Validate(variables))
                    return false;
            }

            return true;
        }

        public bool Matches(VariableMap requiredState)
        {
            foreach (var stateVariable in requiredState)
            {
                if (!ContainsKeyAndValue(stateVariable.Key, stateVariable.Value))
                    return false;
            }

            return true;
        }

        public bool Matches(Dictionary<string, object> contentStateVariables)
        {
            foreach (var stateVariable in contentStateVariables)
            {
                if (!ContainsKeyAndValue(stateVariable.Key, stateVariable.Value))
                    return false;
            }

            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return variables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return variables.GetEnumerator();
        }

        public VariableMap Remove(string key)
        {
            variables.Remove(key);
            TriggerUpdate();
            return this;
        }

        public VariableMap Merge(VariableMap anotherState)
        {
            foreach (var stateVariable in anotherState)
                variables[stateVariable.Key] = stateVariable.Value;

            TriggerUpdate();
            return this;
        }

        public static VariableMap Combine(VariableMap firstState, VariableMap secondState)
        {
            var newState = firstState.Clone();
            newState.Merge(secondState);
            return newState;
        }

        private VariableMap Clone()
        {
            return new VariableMap(variables);
        }

        public object this[string variable]
        {
            get => variables[variable];
            set => variables[variable] = value;
        }

        public void Merge(Dictionary<string, object> dictionary)
        {
            var isChanged = false;
            foreach (var variable in dictionary)
            {
                if (variable.Value == null)
                {
                    variables.Remove(variable.Key);
                    isChanged = true;
                }

                if (!isChanged && variables.TryGetValue(variable.Key, out var currentValue) && Equals(currentValue, variable.Value))
                    continue;

                variables[variable.Key] = variable.Value;
                isChanged = true;
            }

            TriggerUpdate();
        }

        private void TriggerUpdate()
        {
            onUpdated?.Invoke();
        }
    }
}