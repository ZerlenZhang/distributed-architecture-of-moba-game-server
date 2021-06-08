using System;
using System.Collections.Generic;
using ReadyGamerOne.Common;

namespace ReadyGamerOne.MemorySystem
{
    public interface IHotUpdatePath
    {
        string OriginMainManifest { get; }
        string LocalMainPath { get; }
        string WebServeMainPath { get; }   
        string WebServeMainManifest { get; }
        string WebServeVersionPath { get; }
        string WebServeBundlePath { get; }
        string WebServeConfigPath { get; }

        Func<string,string> GetServeConfigPath { get; }
        Func<string,string,string> GetServeBundlePath { get; }
        Func<string,string,string> GetLocalBundlePath { get; }
    }
    
    #region IOriginAssetBundleUtil

    public interface IOriginAssetBundleUtil
    {
        Dictionary<string, string> KeyToName { get; }
        Dictionary<string, string> KeyToPath { get; }
        Dictionary<string,string> NameToPath { get; }
    }    

    public class OriginBundleUtil<T>:
        Singleton<T>,
        IOriginAssetBundleUtil
        where T :OriginBundleUtil<T>,new()
    {
        public virtual Dictionary<string, string> KeyToName => null;
        public virtual Dictionary<string, string> KeyToPath => null;
        public virtual Dictionary<string, string> NameToPath => null;
    }
    #endregion
    
    #region IAssetConstUtil

    public class AssetConstUtil<T> :
        Singleton<T>,
        IAssetConstUtil
        where T :AssetConstUtil<T>, new()
    {
        public virtual Dictionary<string, string> NameToPath
        {
            get { throw new Exception("就不该走到这里"); }
        }
    }
    public interface IAssetConstUtil
    {
        Dictionary<string,string> NameToPath { get; }
    }
    
    #endregion
    
    public class OriginBundleKey
    {
        public const string Self = @"Self";
        public const string Audio = @"Audio";
        public const string File = @"File";
    }
    
}