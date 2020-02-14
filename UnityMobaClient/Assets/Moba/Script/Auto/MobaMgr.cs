using ReadyGamerOne.Script;
using ReadyGamerOne.MemorySystem;
namespace Moba.Script
{
	public partial class MobaMgr : AbstractGameMgr<MobaMgr>
	{
		protected override IResourceLoader ResourceLoader => ResourcesResourceLoader.Instance;
		protected override IAssetConstUtil AssetConstUtil => Utility.AssetConstUtil.Instance;
		partial void OnSafeAwake();
		protected override void Awake()
		{
			base.Awake();
			OnSafeAwake();
		}
	}
}
