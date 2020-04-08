using UnityEngine;
using UnityEngine.Events;

namespace CommonUtils.WebResources {
	public abstract class AbstractWebResourceComponent : MonoBehaviour {
#pragma warning disable 649
		[SerializeField] private string url;
		[SerializeField] private bool loadOnAwake;
		[SerializeField] protected UnityEvent OnResourceReady;
#pragma warning restore 649

		public string Url { get => url; set => url = value; }
		public DownloadStatus Status { get; protected set; } = DownloadStatus.NotInited;

		protected virtual void Awake() {
			if(loadOnAwake) Load();
		}

		public virtual void Load() => Status = DownloadStatus.Loading;
	}
}
