//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: voldemort-client.proto
namespace Voldemort
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ClockEntry")]
  public partial class ClockEntry : global::ProtoBuf.IExtensible
  {
    public ClockEntry() {}
    
    private int _node_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"node_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int node_id
    {
      get { return _node_id; }
      set { _node_id = value; }
    }
    private long _version;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"version", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public long version
    {
      get { return _version; }
      set { _version = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"VectorClock")]
  public partial class VectorClock : global::ProtoBuf.IExtensible
  {
    private readonly global::System.Collections.Generic.List<Voldemort.ClockEntry> _entries = new global::System.Collections.Generic.List<Voldemort.ClockEntry>();
    [global::ProtoBuf.ProtoMember(1, Name=@"entries", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Voldemort.ClockEntry> entries
    {
      get { return _entries; }
    }
  

    private long _timestamp = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"timestamp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long timestamp
    {
      get { return _timestamp; }
      set { _timestamp = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Versioned")]
  public partial class Versioned : global::ProtoBuf.IExtensible
  {
    public Versioned() {}
    
    private byte[] _value;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] value
    {
      get { return _value; }
      set { _value = value; }
    }
    private Voldemort.VectorClock _version;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"version", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public Voldemort.VectorClock version
    {
      get { return _version; }
      set { _version = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Error")]
  public partial class Error : global::ProtoBuf.IExtensible
  {
    public Error() {}
    
    private int _error_code;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"error_code", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int error_code
    {
      get { return _error_code; }
      set { _error_code = value; }
    }
    private string _error_message;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"error_message", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string error_message
    {
      get { return _error_message; }
      set { _error_message = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"KeyedVersions")]
  public partial class KeyedVersions : global::ProtoBuf.IExtensible
  {
    public KeyedVersions() {}
    
    private byte[] _key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] key
    {
      get { return _key; }
      set { _key = value; }
    }
    private readonly global::System.Collections.Generic.List<Voldemort.Versioned> _versions = new global::System.Collections.Generic.List<Voldemort.Versioned>();
    [global::ProtoBuf.ProtoMember(2, Name=@"versions", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Voldemort.Versioned> versions
    {
      get { return _versions; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetRequest")]
  public partial class GetRequest : global::ProtoBuf.IExtensible
  {
    public GetRequest() {}
    

    private byte[] _key = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] key
    {
      get { return _key; }
      set { _key = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetResponse")]
  public partial class GetResponse : global::ProtoBuf.IExtensible
  {
    public GetResponse() {}
    
    private readonly global::System.Collections.Generic.List<Voldemort.Versioned> _versioned = new global::System.Collections.Generic.List<Voldemort.Versioned>();
    [global::ProtoBuf.ProtoMember(1, Name=@"versioned", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Voldemort.Versioned> versioned
    {
      get { return _versioned; }
    }
  

    private Voldemort.Error _error = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"error", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.Error error
    {
      get { return _error; }
      set { _error = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetVersionResponse")]
  public partial class GetVersionResponse : global::ProtoBuf.IExtensible
  {
    public GetVersionResponse() {}
    
    private readonly global::System.Collections.Generic.List<Voldemort.VectorClock> _versions = new global::System.Collections.Generic.List<Voldemort.VectorClock>();
    [global::ProtoBuf.ProtoMember(1, Name=@"versions", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Voldemort.VectorClock> versions
    {
      get { return _versions; }
    }
  

    private Voldemort.Error _error = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"error", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.Error error
    {
      get { return _error; }
      set { _error = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetAllRequest")]
  public partial class GetAllRequest : global::ProtoBuf.IExtensible
  {
    public GetAllRequest() {}
    
    private readonly global::System.Collections.Generic.List<byte[]> _keys = new global::System.Collections.Generic.List<byte[]>();
    [global::ProtoBuf.ProtoMember(1, Name=@"keys", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<byte[]> keys
    {
      get { return _keys; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetAllResponse")]
  public partial class GetAllResponse : global::ProtoBuf.IExtensible
  {
    public GetAllResponse() {}
    
    private readonly global::System.Collections.Generic.List<Voldemort.KeyedVersions> _values = new global::System.Collections.Generic.List<Voldemort.KeyedVersions>();
    [global::ProtoBuf.ProtoMember(1, Name=@"values", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Voldemort.KeyedVersions> values
    {
      get { return _values; }
    }
  

    private Voldemort.Error _error = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"error", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.Error error
    {
      get { return _error; }
      set { _error = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PutRequest")]
  public partial class PutRequest : global::ProtoBuf.IExtensible
  {
    public PutRequest() {}
    
    private byte[] _key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] key
    {
      get { return _key; }
      set { _key = value; }
    }
    private Voldemort.Versioned _versioned;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"versioned", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public Voldemort.Versioned versioned
    {
      get { return _versioned; }
      set { _versioned = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PutResponse")]
  public partial class PutResponse : global::ProtoBuf.IExtensible
  {
    public PutResponse() {}
    

    private Voldemort.Error _error = null;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"error", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.Error error
    {
      get { return _error; }
      set { _error = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DeleteRequest")]
  public partial class DeleteRequest : global::ProtoBuf.IExtensible
  {
    public DeleteRequest() {}
    
    private byte[] _key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public byte[] key
    {
      get { return _key; }
      set { _key = value; }
    }
    private Voldemort.VectorClock _version;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"version", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public Voldemort.VectorClock version
    {
      get { return _version; }
      set { _version = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"DeleteResponse")]
  public partial class DeleteResponse : global::ProtoBuf.IExtensible
  {
    public DeleteResponse() {}
    
    private bool _success;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"success", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool success
    {
      get { return _success; }
      set { _success = value; }
    }

    private Voldemort.Error _error = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"error", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.Error error
    {
      get { return _error; }
      set { _error = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"VoldemortRequest")]
  public partial class VoldemortRequest : global::ProtoBuf.IExtensible
  {
    public VoldemortRequest() {}
    
    private Voldemort.RequestType _type;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public Voldemort.RequestType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private bool _should_route;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"should_route", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public bool should_route
    {
      get { return _should_route; }
      set { _should_route = value; }
    }
    private string _store;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"store", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string store
    {
      get { return _store; }
      set { _store = value; }
    }

    private Voldemort.GetRequest _get = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"get", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.GetRequest get
    {
      get { return _get; }
      set { _get = value; }
    }

    private Voldemort.GetAllRequest _getAll = null;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"getAll", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.GetAllRequest getAll
    {
      get { return _getAll; }
      set { _getAll = value; }
    }

    private Voldemort.PutRequest _put = null;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"put", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.PutRequest put
    {
      get { return _put; }
      set { _put = value; }
    }

    private Voldemort.DeleteRequest _delete = null;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"delete", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Voldemort.DeleteRequest delete
    {
      get { return _delete; }
      set { _delete = value; }
    }

    private int _requestRouteType = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"requestRouteType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int requestRouteType
    {
      get { return _requestRouteType; }
      set { _requestRouteType = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"RequestType")]
    public enum RequestType
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"GET", Value=0)]
      GET = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GET_ALL", Value=1)]
      GET_ALL = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"PUT", Value=2)]
      PUT = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DELETE", Value=3)]
      DELETE = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"GET_VERSION", Value=4)]
      GET_VERSION = 4
    }
  
}