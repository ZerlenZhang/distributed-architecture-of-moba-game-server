//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: protos/LoginProtocol.proto
namespace PurificationPioneer.Network.ProtoGen
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserUnregisterRes")]
  public partial class UserUnregisterRes : global::ProtoBuf.IExtensible
  {
    public UserUnregisterRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EditProfileReq")]
  public partial class EditProfileReq : global::ProtoBuf.IExtensible
  {
    public EditProfileReq() {}
    
    private string _unick;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"unick", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string unick
    {
      get { return _unick; }
      set { _unick = value; }
    }
    private int _uface;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uface", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uface
    {
      get { return _uface; }
      set { _uface = value; }
    }
    private int _usex;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"usex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int usex
    {
      get { return _usex; }
      set { _usex = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EditProfileRes")]
  public partial class EditProfileRes : global::ProtoBuf.IExtensible
  {
    public EditProfileRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserAccountInfo")]
  public partial class UserAccountInfo : global::ProtoBuf.IExtensible
  {
    public UserAccountInfo() {}
    
    private int _uid;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private string _uname;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uname", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string uname
    {
      get { return _uname; }
      set { _uname = value; }
    }
    private string _pwd;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"pwd", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string pwd
    {
      get { return _pwd; }
      set { _pwd = value; }
    }
    private string _unick;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"unick", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string unick
    {
      get { return _unick; }
      set { _unick = value; }
    }
    private int _ulevel;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"ulevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ulevel
    {
      get { return _ulevel; }
      set { _ulevel = value; }
    }
    private int _uexp;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"uexp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uexp
    {
      get { return _uexp; }
      set { _uexp = value; }
    }
    private int _urank;
    [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name=@"urank", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int urank
    {
      get { return _urank; }
      set { _urank = value; }
    }
    private int _ucoin;
    [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name=@"ucoin", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ucoin
    {
      get { return _ucoin; }
      set { _ucoin = value; }
    }
    private int _udiamond;
    [global::ProtoBuf.ProtoMember(9, IsRequired = true, Name=@"udiamond", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int udiamond
    {
      get { return _udiamond; }
      set { _udiamond = value; }
    }
    private string _usignature;
    [global::ProtoBuf.ProtoMember(10, IsRequired = true, Name=@"usignature", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string usignature
    {
      get { return _usignature; }
      set { _usignature = value; }
    }
    private int _uintegrity;
    [global::ProtoBuf.ProtoMember(11, IsRequired = true, Name=@"uintegrity", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uintegrity
    {
      get { return _uintegrity; }
      set { _uintegrity = value; }
    }
    private int _status;
    [global::ProtoBuf.ProtoMember(12, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserLoginReq")]
  public partial class UserLoginReq : global::ProtoBuf.IExtensible
  {
    public UserLoginReq() {}
    
    private string _uname;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"uname", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string uname
    {
      get { return _uname; }
      set { _uname = value; }
    }
    private string _pwd;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"pwd", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string pwd
    {
      get { return _pwd; }
      set { _pwd = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserLoginRes")]
  public partial class UserLoginRes : global::ProtoBuf.IExtensible
  {
    public UserLoginRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private UserAccountInfo _uinfo = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"uinfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public UserAccountInfo uinfo
    {
      get { return _uinfo; }
      set { _uinfo = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}