using System;
using System.Collections;
using System.Collections.Generic;
using Plugins.UnityMonstackCore.Extensions.Collections;
using Plugins.UnityMonstackCore.Loggers;

namespace Plugins.Shared.UnityMonstackCore.Utils
{
    public class VariableMap : IEnumerable<KeyValuePair<string, object>>
    {
        protected Dictionary<string, object> variables = new Dictionary<string, object>();

        public VariableMap()
        {
        }

        public VariableMap(Dictionary<string, object> variables)
        {
            foreach (var keyValuePair in variables)
                this.variables[keyValuePair.Key] = keyValuePair.Value;
        }

        public VariableMap SetVariable(string key, object value)
        {
            variables[key] = value;
            return this;
        }

        public VariableMap SetOrModifyVariable<T>(string key, T value, Func<T, T, T> modifyLambda) where T : class
        {
            if (variables.ContainsKey(key))
                variables[key] = modifyLambda.Invoke(variables[key] as T, value);
            else
                variables[key] = value;

            return this;
        }

        public object GetVariable(string key)
        {
            return variables[key];
        }

        public T GetVariable<T>(string key)
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

        public bool HasVariable(string key, object value)
        {
            return variables.ContainsKey(key) && variables[key].Equals(value);
        }

        public T GetVariable<T>(string key, T defaultValue) 
        {
            try
            {
                return (T)variables.GetValueOrDefault(key, defaultValue);
            }
            catch (InvalidCastException e)
            {
                UnityLogger.Error($"Failed to get state variable as {typeof(T).Name}, whereas it's {variables[key].GetType().Name}", e);
                throw;
            }
        }

        public bool MatchesRequiredState(VariableMap requiredState)
        {
            foreach (var stateVariable in requiredState)
            {
                if (!HasVariable(stateVariable.Key, stateVariable.Value))
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

        public VariableMap Merge(VariableMap anotherState)
        {
            foreach (var stateVariable in anotherState)
                SetVariable(stateVariable.Key, stateVariable.Value);

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
    }
}