using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
				/*if (MemberInfo.ReturnType.IsGenericType)
				{
					invokeResult = (object)await (dynamic)MemberInfo.Invoke(instance, arguments);
				}
				else
				{
					await (Task)MemberInfo.Invoke(instance, arguments);
				}*/
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
	}
}