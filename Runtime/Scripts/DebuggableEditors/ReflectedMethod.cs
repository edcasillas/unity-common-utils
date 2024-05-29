using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace CommonUtils.DebuggableEditors {
	public class ReflectedMethod : AbstractReflectedMember<MethodInfo> {
		public bool HasParameters { get; }
		public bool HasOutParameters { get; }
		public ParameterInfo[] ParamInfo { get; }
		public object[] Arguments { get; }
		public bool IsAwaitable { get; }

		public bool HasBeenCalled { get; private set; }
		public bool FinishedExecuting { get; private set; }
		public bool HasReturnValue => MemberInfo.ReturnType != typeof(void);

		private readonly Stopwatch stopwatch = new Stopwatch();
		public Stopwatch StopWatch => stopwatch;

		public override Type Type => MemberInfo.ReturnType;

		public object ReturnValue { get; private set; }

		public ReflectedMethod(MethodInfo methodInfo, string displayName) : base(methodInfo, displayName) {
			ParamInfo = methodInfo.GetParameters();
			HasParameters = ParamInfo.Any();
			HasOutParameters = ParamInfo.Any(p => p.IsOut);
			Arguments = ParamInfo.Select(p => p.DefaultValue == DBNull.Value ? null : p.DefaultValue).ToArray();

			IsAwaitable = methodInfo.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
		}

		public void Invoke(object instance) {
			object invokeResult = null;
			if (IsAwaitable) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				invokeAsync(instance);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				return;
			}
			else {
				if (MemberInfo.ReturnType == typeof(void)) {
					MemberInfo.Invoke(instance, Arguments);
				}
				else {
					invokeResult = MemberInfo.Invoke(instance, Arguments);
				}

				FinishedExecuting = true;
			}

			HasBeenCalled = true;
			ReturnValue = invokeResult;
		}

		private async Task invokeAsync(object instance) {
			// Assuming the method is awaitable
			object invokeResult = null;

			HasBeenCalled = true;
			FinishedExecuting = false;

			var returnType = MemberInfo.ReturnType;

			if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)) {
				// If return type is Task<T>
				var task = (Task)MemberInfo.Invoke(instance, Arguments);
				await task.ConfigureAwait(false);
				var resultProperty = returnType.GetProperty("Result");
				invokeResult = resultProperty?.GetValue(task);
			} else if (returnType == typeof(Task)) {
				// If return type is Task
				var task = (Task)MemberInfo.Invoke(instance, Arguments);
				await task.ConfigureAwait(false);
			} else {
				// If return type is not Task or Task<T>
				throw new InvalidOperationException("Method is not awaitable");
			}

			FinishedExecuting = true;
			ReturnValue = invokeResult;
		}

		/*public async Task InvokeAsync(object instance) {
			// Assuming the method is awaitable
			object invokeResult = null;

			HasBeenCalled = true;
			FinishedExecuting = false;
			if (MemberInfo.ReturnType.IsGenericType) { // return type is of type Task<T> (i.e. generic)
				invokeResult = (object)await (dynamic)MemberInfo.Invoke(instance, Arguments);
			} else { // return type is Task (non generic, no return value)
				await (Task)MemberInfo.Invoke(instance, Arguments);
			}

			FinishedExecuting = true;
			ReturnValue = invokeResult;
		}*/
	}
}