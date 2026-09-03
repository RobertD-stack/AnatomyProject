/*************************************************************************
 *
 * Copyright 2026, Mechdyne Corporation
 * ALL RIGHTS RESERVED
 *
 * UNPUBLISHED -- Rights reserved under the copyright laws of the United
 * States. Use of a copyright notice is precautionary only and does not
 * imply publication or disclosure.
 *
 * THE CONTENT OF THIS WORK CONTAINS CONFIDENTIAL AND PROPRIETARY
 * INFORMATION OF MECHDYNE CORPORATION. ANY DUPLICATION, MODIFICATION,
 * DISTRIBUTION, OR DISCLOSURE IN ANY FORM, IN WHOLE, OR IN PART, IS
 * STRICTLY PROHIBITED WITHOUT THE PRIOR EXPRESS WRITTEN PERMISSION OF
 * MECHDYNE CORPORATION.
 *
 * Version 4.5.0.4237
 *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// The getReal3D.RpcManager takes care of sending RPC calls.
    /// </summary>
    public class RpcManager : MonoBehaviour
    {

        internal static getReal3D.LogLevel logLevel
        {
            get
            {
                return Settings.get().rpcLogLevel;
            }
        }

        internal static IntPtr logger
        {
            get
            {
                if (Application.isEditor)
                {
                    return default(IntPtr);
                }
                if (m_logger == default(IntPtr))
                {
                    m_logger = Plugin.getNativePlugin().createLogger("RPC");
                }
                return m_logger;
            }
        }

        private static IntPtr m_logger;

        private static RpcManager instance;

        internal static void Init()
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find("GetRealManager");
                if (go == null)
                    go = new GameObject("GetRealManager");
                DontDestroyOnLoad(go);
                go.AddComponent<RpcManager>();
                instance = go.GetComponent<RpcManager>() as RpcManager;
            }
        }

        void OnApplicationQuit()
        {
            instance = null;
        }

        void Update()
        {
            flushRpcCalls();
        }

        private struct RpcCall
        {
            public MonoBehaviourWithRpc obj;
            public int methodId;
            public System.Reflection.MethodInfo method
            {
                get { return rpcInfo.method; }
            }
            public MonoBehaviourWithRpc.RpcInfo rpcInfo
            {
                get { return obj.methods[methodId]; }
            }
            public object[] arguments;
            public void invoke()
            {
                method.Invoke(obj, arguments);
            }

        }

        static private WriteBuffer m_writeBuffer = new WriteBuffer();
        static private ReadBuffer m_readBuffer = new ReadBuffer();
        static private List<RpcCall> m_rpcCalls = new List<RpcCall>();
        static private List<RpcCall> m_lastFrameCalls = new List<RpcCall>();
        static private Dictionary<int, MonoBehaviourWithRpc> m_behaviourIds =
            new Dictionary<int, MonoBehaviourWithRpc>();
        static private HashSet<MonoBehaviourWithRpc> m_registeredObjects = new
            HashSet<MonoBehaviourWithRpc>();

        static private int m_behaviourIdCounter = 0;

        internal static void registerRpcMethod(MonoBehaviourWithRpc obj, int methodId, MonoBehaviourWithRpc.RpcInfo rpcInfo)
        {
            Plugin.Log(logLevel, "Registering RPC method " + methodId.ToString() + " [" + obj.
                GetInstanceID() + "] " + obj.name + "." + rpcInfo.method.Name, logger);
            try
            {
                if (!obj.rpcTargetId.HasValue)
                {
                    obj.rpcTargetId = ++m_behaviourIdCounter;
                    Plugin.Log(logLevel, "Registering RPC target object " + obj.name + " with ID "
                        + obj.rpcTargetId.Value, logger);
                    m_behaviourIds.Add(obj.rpcTargetId.Value, obj);
                }


            }
            catch (System.ArgumentException)
            {
                Plugin.error("An RPC with the name " + rpcInfo.method.Name +
                    " has already been registered on object " + obj.name + ".", logger);
            }
            m_registeredObjects.Add(obj);
        }

        internal static void deregisterRpcMethods(MonoBehaviourWithRpc obj)
        {
            Plugin.Log(logLevel, "De-registering RPC target object " + " [" + obj.
                GetInstanceID() + "] " + obj.name + " with ID " +
                obj.rpcTargetId.Value + ".", logger);
            if (!m_registeredObjects.Remove(obj))
            {
                Plugin.error("deregisterRpcMethod target hasn't been registered!", logger);
                return;
            }
        }

        /// <summary>
        /// Add an RPC to the call list.
        /// </summary>
        [Obsolete("This method is obsolete. Call MonoBehaviourWithRpc.CallRpc instead.")]
        static public void call(string methodName, params object[] arguments)
        {
            var potentialTargets = m_registeredObjects.
                Where(o => o.methods.Any(m => m.method.Name == methodName));

            if (potentialTargets.Count() == 0)
            {
                Plugin.error("No registered RPC target object has an RPC named '" +
                    methodName + "'.", logger);
            }
            else if (potentialTargets.Count() != 1)
            {
                Plugin.error("Multiple registered RPC target object have an RPC named '" +
                    methodName + "'. Use MonoBehaviourWithRpc.CallRpc instead of RpcManager.call " +
                    "in order to specify the target.", logger);
            }
            else
            {
                potentialTargets.First().CallRpc(methodName, arguments);
            }
        }

        internal static void CallRpc(MonoBehaviourWithRpc obj, int methodId,
            params object[] arguments)
        {
            if (obj == null)
            {
                Plugin.error("RPC target is null.", logger);
            }
            if (methodId < 0 || methodId >= obj.methods.Length)
            {
                Plugin.error("RPC method is null.", logger);
            }

            RpcCall rpcCall;
            rpcCall.obj = obj;
            rpcCall.arguments = cloneArguments(arguments);
            rpcCall.methodId = methodId;

            logRpcCall("Enqueuing RPC call ", rpcCall);

            m_rpcCalls.Add(rpcCall);
        }

        internal static object[] cloneArguments(object[] arguments)
        {
            object[] res = arguments.Clone() as object[];
            for (var i = 0; i < res.Length; ++i)
            {
                if (res[i] is byte[])
                {
                    var asByteArray = (byte[])res[i];
                    res[i] = asByteArray.Clone();
                }
            }
            return res;
        }

        private static void flushRpcCalls()
        {
            if (Plugin.getClusterID() == 0)
            {
                executeCalls(m_lastFrameCalls);

                SerializeCalls(m_rpcCalls, m_writeBuffer);
                Plugin.setNextFramePayload(m_writeBuffer);

                // test deserialization in the editor
                if (Application.isEditor)
                {
                    m_readBuffer.Resize(m_writeBuffer.buffer.Length);
                    m_writeBuffer.buffer.CopyTo(m_readBuffer.buffer, 0);
                    m_lastFrameCalls.Clear();
                    m_rpcCalls.Clear();
                    DeserializeCalls(m_readBuffer, m_lastFrameCalls);
                }
                else
                {
                    List<RpcCall> temp = m_lastFrameCalls;
                    m_lastFrameCalls = m_rpcCalls;
                    m_rpcCalls = temp;
                    m_rpcCalls.Clear();
                }

                // Client -> master RPC calls
                var clientCount = Plugin.getConfigCount() - 1;
                for (uint id = 0; id < (uint)clientCount; ++id)
                {
                    Plugin.getClientPayload(id, m_readBuffer);
                    if (m_readBuffer.availableBytes != 0)
                    {
                        List<RpcCall> calls = new();
                        DeserializeCalls(m_readBuffer, calls);
                        executeCalls(calls);
                    }
                }
            }
            else
            {
                Plugin.getPayload(m_readBuffer);
                if (m_readBuffer.availableBytes > 0)
                {
                    DeserializeCalls(m_readBuffer, m_lastFrameCalls);
                    executeCalls(m_lastFrameCalls);
                    m_lastFrameCalls.Clear();
                }

                if (m_rpcCalls.Count != 0)
                {
                    SerializeCalls(m_rpcCalls, m_writeBuffer);
                    Plugin.setNextFramePayload(m_writeBuffer);
                    m_rpcCalls.Clear();
                }
            }
        }

        private static void DeserializeCalls(ReadBuffer buffer, List<RpcCall> rpcCalls)
        {
            if (buffer.availableBytes == 0)
            {
                Plugin.error("RPC buffer is empty!");
                return;
            }

            int count = 0;
            buffer.Read(ref count);

            for (int i = 0; i < count; ++i)
            {
                RpcCall rpcCall = new RpcCall();
                rpcCall.methodId = 0;
                int objectId = 0;
                buffer.Read(ref objectId);
                buffer.Read(ref rpcCall.methodId);
                try
                {
                    rpcCall.obj = m_behaviourIds[objectId];
                    rpcCall.arguments = readArguments(buffer, rpcCall.rpcInfo);
                    rpcCalls.Add(rpcCall);
                }
                catch (KeyNotFoundException)
                {
                    Plugin.error("Target with ID " + objectId + " doesn't exist anymore.", logger);
                }
            }
        }

        private static void SerializeCalls(List<RpcCall> calls, WriteBuffer buffer)
        {
            buffer.Reset();
            buffer.WriteInteger(calls.Count);

            foreach (var call in calls)
            {
                buffer.WriteInteger(call.obj.rpcTargetId.Value);
                buffer.WriteInteger(call.methodId);
                writeArguments(buffer, call);
            }
        }

        private static void executeCalls(List<RpcCall> calls)
        {
            int count = calls.Count;
            for (int i = 0; i < count; ++i)
            {
                var call = calls[i];
                try
                {
                    if (m_registeredObjects.Contains(call.obj))
                    {
                        logRpcCall("Invoking RPC call ", call);
                        call.invoke();
                    }
                    else
                    {
                        Plugin.error("Not invoking RPC call "
                            + call.method.Name + ": target object has been deleted.", logger);
                    }
                }
                catch (System.Reflection.TargetParameterCountException)
                {
                    int expectedArgCount = call.method.GetParameters().Length;
                    int argCount = call.arguments.Length;
                    Plugin.error("Error while invoking RPC call " + call.method.Name + ": got "
                        + argCount.ToString() + " argument(s) while we expected " +
                        expectedArgCount.ToString() + ".", logger);
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    Plugin.error("Error while invoking RPC call " + call.method.Name +
                        ": " + e.InnerException.ToString());
                    Debug.LogException(e.InnerException, call.obj);
                }
                catch (System.ArgumentException)
                {
                    Plugin.error("Error while invoking RPC call " + call.method.Name +
                        ": unable to convert parameters.", logger);
                }
                catch (System.Exception e)
                {
                    Plugin.error("Exception caught during RPC call " + call.method.Name + ". "
                        + e.Message, logger);
                }
            }
        }

        private static object[] readArguments(ReadBuffer stream, MonoBehaviourWithRpc.RpcInfo rpcInfo)
        {
            int count = 0;
            stream.Read(ref count);
            object[] arguments = new object[count];
            for (int i = 0; i < count; ++i)
            {
                int type = 0;
                stream.Read(ref type);
                if (rpcInfo.parameters[i].serializer != null)
                {
                    Debug.Assert(type == -1);
                    byte[] bytes = null;
                    stream.Read(ref bytes);
                    arguments[i] = rpcInfo.parameters[i].serializer.deserialize(bytes);
                }
                else if (type == 1)
                {
                    bool o = false;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 2)
                {
                    byte o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 3)
                {
                    char o = 'a';
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 4)
                {
                    short o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 5)
                {
                    int o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 6)
                {
                    uint o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 7)
                {
                    string o = "";
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 8)
                {
                    float o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 9)
                {
                    Vector2 o = Vector2.zero;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 10)
                {
                    Vector3 o = Vector3.zero;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 11)
                {
                    Quaternion o = Quaternion.identity;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 12)
                {
                    Color o = Color.red;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 13)
                {
                    Color32 o = new Color32();
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 14)
                {
                    long o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 15)
                {
                    ulong o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 16)
                {
                    ushort o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 17)
                {
                    double o = 0;
                    stream.Read(ref o);
                    arguments[i] = o;
                }
                else if (type == 18)
                {
                    byte[] bytes = null;
                    stream.Read(ref bytes);
                    arguments[i] = bytes;
                }
                else if (type == 19)
                {
                    byte[] bytes = null;
                    stream.Read(ref bytes);
                    using (var memoryStream = new MemoryStream(bytes))
                    {
                        var serializer = new BinaryFormatter();
                        arguments[i] = serializer.Deserialize(memoryStream);
                    }
                }
                else
                {
                    Plugin.error("Could not deserialize type " + type.ToString()
                        + " in RPC call", logger);
                }
            }
            return arguments;
        }

        private static void writeArguments(WriteBuffer stream, RpcCall call)
        {
            if (call.arguments.Length != call.rpcInfo.parameters.Length)
            {
                throw new Exception($"RPC call {call.method} is expecting {call.rpcInfo.parameters.Length} arguments and not {call.arguments.Length}.");
            }
            var args = call.arguments;
            int count = args.Length;
            stream.WriteInteger(count);
            for (int i = 0; i < count; ++i)
            {

                object obj = args[i];
                if (obj is Enum)
                {
                    Enum e = (Enum)obj;
                    obj = Convert.ChangeType(e, e.GetTypeCode());
                }
                if (call.rpcInfo.parameters[i].serializer != null)
                {
                    byte[] bytes = call.rpcInfo.parameters[i].serializer.serialize(obj);
                    stream.WriteInteger(-1);
                    stream.WriteBytes(bytes);
                }
                else if (obj is bool)
                {
                    stream.WriteInteger(1);
                    stream.WriteBool((bool)obj);
                }
                else if (obj is byte)
                {
                    stream.WriteInteger(2);
                    stream.WriteByte((byte)obj);
                }
                else if (obj is char)
                {
                    stream.WriteInteger(3);
                    stream.WriteChar((char)obj);
                }
                else if (obj is short)
                {
                    stream.WriteInteger(4);
                    stream.WriteShort((short)obj);
                }
                else if (obj is int)
                {
                    stream.WriteInteger(5);
                    stream.WriteInteger((int)obj);
                }
                else if (obj is uint)
                {
                    stream.WriteInteger(6);
                    stream.WriteUInteger((uint)obj);
                }
                else if (obj is string)
                {
                    stream.WriteInteger(7);
                    stream.WriteString((string)obj);
                }
                else if (obj is float)
                {
                    stream.WriteInteger(8);
                    stream.WriteFloat((float)obj);
                }
                else if (obj is Vector2)
                {
                    stream.WriteInteger(9);
                    stream.WriteVector2((Vector2)obj);
                }
                else if (obj is Vector3)
                {
                    stream.WriteInteger(10);
                    stream.WriteVector3((Vector3)obj);
                }
                else if (obj is Quaternion)
                {
                    stream.WriteInteger(11);
                    stream.WriteQuaternion((Quaternion)obj);
                }
                else if (obj is Color)
                {
                    stream.WriteInteger(12);
                    stream.WriteColor((Color)obj);
                }
                else if (obj is Color32)
                {
                    stream.WriteInteger(13);
                    stream.WriteColor((Color)obj);
                }
                else if (obj is long)
                {
                    stream.WriteInteger(14);
                    stream.WriteLong((long)obj);
                }
                else if (obj is ulong)
                {
                    stream.WriteInteger(15);
                    stream.WriteULong((ulong)obj);
                }
                else if (obj is ushort)
                {
                    stream.WriteInteger(16);
                    stream.WriteUShort((ushort)obj);
                }
                else if (obj is double)
                {
                    stream.WriteInteger(17);
                    stream.WriteDouble((double)obj);
                }
                else if (obj is byte[])
                {
                    stream.WriteInteger(18);
                    stream.WriteBytes((byte[])obj);
                }
                else if (call.rpcInfo.parameters[i].type.IsSerializable)
                {
                    stream.WriteInteger(19);
                    using (var memoryStream = new MemoryStream())
                    {
                        var serializer = new BinaryFormatter();
                        serializer.Serialize(memoryStream, obj);
                        stream.WriteBytes(memoryStream.ToArray());
                    }
                }
                else
                {
                    Plugin.error("Could not serialize type " + obj.GetType().Name
                        + " in RPC call", logger);
                }
            }
        }

        private static void logRpcCall(String prefix, RpcCall rpcCall)
        {
            if (logLevel != LogLevel.Discarded)
            {
                StringBuilder ss = new StringBuilder();
                ss
                    .Append(prefix)
                    .Append("[")
                    .Append(rpcCall.obj.GetInstanceID())
                    .Append("] ")
                    .Append(rpcCall.obj.name)
                    .Append(".")
                    .Append(rpcCall.method.Name)
                    .Append("(");

                for (int i = 0; i < rpcCall.arguments.Length; ++i)
                {
                    if (i != 0)
                    {
                        ss.Append(", ");
                    }
                    if (rpcCall.arguments[i] == null)
                    {
                        ss.Append("null");
                    }
                    else if (rpcCall.arguments[i] is string)
                    {
                        ss.Append('"');
                        ss.Append(rpcCall.arguments[i].ToString());
                        ss.Append('"');
                    }
                    else
                    {
                        ss.Append(rpcCall.arguments[i].ToString());
                    }
                }
                ss.Append(").");
                Plugin.Log(logLevel, ss.ToString(), logger);
            }
        }

    }

    /// <summary>
    /// Attribute for an RPC method to specify a custom serializer.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false)]
    public class RPCSerializer : System.Attribute
    {
        internal Type SerializerType { get; }
        internal Type SerializedType { get; }
        internal MethodInfo SerializeMethod { get; }
        internal MethodInfo DeserializeMethod { get; }
        internal object SerializerInstance { get; }

        /// <summary>
        /// Instantiate an RPCSerializer with the given serializer type.
        /// </summary>
        public RPCSerializer(Type serializer)
        {
            foreach (var iface in serializer.GetInterfaces())
            {
                if (iface.GetGenericTypeDefinition() == typeof(IRPCSerializer<>))
                {
                    SerializerType = serializer;
                    SerializedType = iface.GetGenericArguments().First();
                    var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
                    var methods = SerializerType.GetMethods(flags);
                    SerializeMethod = methods.Where(m => m.Name == nameof(IRPCSerializer<int>.serialize)).First();
                    DeserializeMethod = methods.Where(m => m.Name == nameof(IRPCSerializer<int>.deserialize)).First();
                    SerializerInstance = Activator.CreateInstance(SerializerType);
                    break;
                }
            }
            if (SerializerType == null || SerializedType == null)
            {
                throw new Exception($"RPCSerializer type {serializer} doesn't implement ISerializer<>.");
            }
        }

        internal object deserialize(byte[] bytes)
        {
            return DeserializeMethod.Invoke(SerializerInstance, new object[] { bytes });
        }
        internal byte[] serialize(object obj)
        {
            if (obj.GetType() != SerializedType)
            {
                throw new Exception($"RPCSerializer {SerializerType} was given type {obj.GetType()} to serialize.");
            }
            return (byte[])SerializeMethod.Invoke(SerializerInstance, new object[] { obj });
        }
    }

    /// <summary>
    /// Serializing interface that needs to be implemented when providing a custom
    /// object serializer via RPCSerializer attribute.
    /// </summary>
    public interface IRPCSerializer<T>
    {
        /// <summary>
        /// Serialize the given object into a byte array.
        /// </summary>
        byte[] serialize(T obj);

        /// <summary>
        /// Deserialize the given byte array into an object.
        /// </summary>
        T deserialize(byte[] bytes);
    }

    /// <summary>
    /// Attribute for a callable RPC method.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class RPC : System.Attribute { }

    /// <summary>
    /// MonoBehaviour with automatic registration of RPC methods.
    /// </summary>
    abstract public class MonoBehaviourWithRpc : MonoBehaviour
    {
        internal int? rpcTargetId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MonoBehaviourWithRpc()
        {
            m_autoRegister = true;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="autoRegister">If true, then the RPC will be automatically
        /// registered when the OnEnable event is called.</param>
        public MonoBehaviourWithRpc(bool autoRegister)
        {
            m_autoRegister = autoRegister;
        }

        /// <summary>
        /// Calls deregisterRpc when this object gets disabled.
        /// </summary>
        public virtual void OnDisable()
        {
            Plugin.Log(RpcManager.logLevel, GetType().ToString() + " " + name + "[" +
                GetInstanceID() + "] disabled.", RpcManager.logger);
            deregisterRpc();
        }

        /// <summary>
        /// Calls registerRpc when this object gets enabled.
        /// </summary>
        public virtual void OnEnable()
        {
            Plugin.Log(RpcManager.logLevel, GetType().ToString() + " " + name + " [" +
                GetInstanceID() + "] enabled.", RpcManager.logger);
            if (m_autoRegister)
            {
                registerRpc();
            }
        }

        /// <summary>
        /// Call an RPC on this object.
        /// </summary>
        /// <param name="methodName">The method to call</param>
        /// <param name="arguments">Arguments</param>
        public void CallRpc(string methodName, params object[] arguments)
        {
            if (!Cluster.isMaster)
            {
                throw new Exception("RPC must be made from master!");
            }
            CallRpcUnchecked(methodName, arguments);
        }

        /// <summary>
        /// Call a master RPC on this object.
        /// </summary>
        /// <param name="methodName">The method to call</param>
        /// <param name="arguments">Arguments</param>
        public void CallMasterRpc(string methodName, params object[] arguments)
        {
            if (Cluster.isMaster)
            {
                throw new Exception("Master RPC must be made from clients!");
            }
            CallRpcUnchecked(methodName, arguments);
        }

        private void CallRpcUnchecked(string methodName, params object[] arguments)
        {
            int methodId = -1;
            for (int i = 0; i < m_methods.Length && methodId == -1; ++i)
            {
                if (m_methods[i].method.Name == methodName)
                {
                    methodId = i;
                }
            }
            if (methodId >= 0)
            {
                RpcManager.CallRpc(this, methodId, arguments);
            }
            else
            {
                throw new Exception(string.Format("Unable to find a method called {0} on object {1}", methodName, GetType().FullName));
            }
        }

        /// <summary>
        /// Register all RPC methods found in that object.
        /// </summary>
        protected void registerRpc()
        {
            Plugin.Log(RpcManager.logLevel, GetType().ToString() + " " + name + " [" +
                GetInstanceID() + "] Registering RPCs.", RpcManager.logger);
            if (m_registered)
            {
                Plugin.error("RPC have already been registered.", RpcManager.logger);
                return;
            }
            var rpcs = getRpcList();
            m_methods = rpcs.ToArray();
            if (rpcs.Length == 0)
            {
                m_registered = false;
                Plugin.Log(RpcManager.logLevel, GetType().ToString() + " " + name + " [" +
                    GetInstanceID() + "] No need to register since no RPC is defined.",
                    RpcManager.logger);
            }
            else
            {
                for (int i = 0; i < rpcs.Length; ++i)
                {
                    RpcManager.registerRpcMethod(this, i, rpcs[i]);
                }
                m_registered = true;
            }
        }

        /// <summary>
        /// Deregister all RPC methods found in that object.
        /// </summary>
        protected void deregisterRpc()
        {
            Plugin.Log(RpcManager.logLevel, GetType().ToString() + " " + name + " [" +
                GetInstanceID() + "] De-registering RPCs.", RpcManager.logger);
            if (!m_registered)
            {
                return;
            }
            RpcManager.deregisterRpcMethods(this);
            m_registered = false;
        }

        private bool m_registered = false;
        private bool m_autoRegister = true;

        internal struct ParameterInfo
        {
            public RPCSerializer serializer;
            public Type type;
        }

        internal struct RpcInfo
        {
            public int methodId;
            public System.Reflection.MethodInfo method;
            public ParameterInfo[] parameters;
        }

        private RpcInfo[] m_methods;
        internal RpcInfo[] methods
        {
            get { return m_methods; }
        }

        private RpcInfo[] getRpcList()
        {
            System.Reflection.MethodInfo[] methods = GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);

            var res = new List<RpcInfo>();
            for (int i = 0; i < methods.Length; ++i)
            {
                var method = methods[i];
                object[] attributes = method.GetCustomAttributes(typeof(RPC), true);
                if (attributes.Length == 1)
                {
                    var parameters = method.GetParameters();
                    var parameterInfo = new ParameterInfo[parameters.Length];
                    for (int j = 0; j < parameters.Length; ++j)
                    {
                        var parameter = parameters[j];
                        parameterInfo[j].type = parameter.ParameterType;
                        foreach (var attribute in parameter.GetCustomAttributes(false))
                        {
                            if (attribute is RPCSerializer rpcSerializer)
                            {
                                parameterInfo[j].serializer = rpcSerializer;
                            }
                        }
                    }

                    var rpcInfo = new RpcInfo
                    {
                        methodId = i,
                        method = method,
                        parameters = parameterInfo
                    };
                    res.Add(rpcInfo);
                }
            }
            return res.ToArray();
        }
    }
}
