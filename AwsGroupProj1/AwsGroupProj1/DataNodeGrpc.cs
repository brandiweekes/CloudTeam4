// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: DataNode.proto
// </auto-generated>
#pragma warning disable 1591
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using grpc = global::Grpc.Core;

namespace Sufs {
  public static partial class FileHandling
  {
    static readonly string __ServiceName = "sufs.FileHandling";

    static readonly grpc::Marshaller<global::Sufs.WriteRequest> __Marshaller_WriteRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.WriteRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.WriteResponse> __Marshaller_WriteResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.WriteResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse> __Method_WriteFile = new grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "WriteFile",
        __Marshaller_WriteRequest,
        __Marshaller_WriteResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Sufs.DataNodeReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of FileHandling</summary>
    public abstract partial class FileHandlingBase
    {
      public virtual global::System.Threading.Tasks.Task<global::Sufs.WriteResponse> WriteFile(global::Sufs.WriteRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for FileHandling</summary>
    public partial class FileHandlingClient : grpc::ClientBase<FileHandlingClient>
    {
      /// <summary>Creates a new client for FileHandling</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public FileHandlingClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for FileHandling that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public FileHandlingClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected FileHandlingClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected FileHandlingClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::Sufs.WriteResponse WriteFile(global::Sufs.WriteRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return WriteFile(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sufs.WriteResponse WriteFile(global::Sufs.WriteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_WriteFile, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.WriteResponse> WriteFileAsync(global::Sufs.WriteRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return WriteFileAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.WriteResponse> WriteFileAsync(global::Sufs.WriteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_WriteFile, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override FileHandlingClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new FileHandlingClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(FileHandlingBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_WriteFile, serviceImpl.WriteFile).Build();
    }

  }
  public static partial class DataNodeHealthCenter
  {
    static readonly string __ServiceName = "sufs.DataNodeHealthCenter";

    static readonly grpc::Marshaller<global::Sufs.HealthRequest> __Marshaller_HealthRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.HealthRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.HealthResponse> __Marshaller_HealthResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.HealthResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Sufs.HealthRequest, global::Sufs.HealthResponse> __Method_HealthCheck = new grpc::Method<global::Sufs.HealthRequest, global::Sufs.HealthResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "HealthCheck",
        __Marshaller_HealthRequest,
        __Marshaller_HealthResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Sufs.DataNodeReflection.Descriptor.Services[1]; }
    }

    /// <summary>Base class for server-side implementations of DataNodeHealthCenter</summary>
    public abstract partial class DataNodeHealthCenterBase
    {
      public virtual global::System.Threading.Tasks.Task<global::Sufs.HealthResponse> HealthCheck(grpc::IAsyncStreamReader<global::Sufs.HealthRequest> requestStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for DataNodeHealthCenter</summary>
    public partial class DataNodeHealthCenterClient : grpc::ClientBase<DataNodeHealthCenterClient>
    {
      /// <summary>Creates a new client for DataNodeHealthCenter</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public DataNodeHealthCenterClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for DataNodeHealthCenter that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public DataNodeHealthCenterClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected DataNodeHealthCenterClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected DataNodeHealthCenterClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual grpc::AsyncClientStreamingCall<global::Sufs.HealthRequest, global::Sufs.HealthResponse> HealthCheck(grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return HealthCheck(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncClientStreamingCall<global::Sufs.HealthRequest, global::Sufs.HealthResponse> HealthCheck(grpc::CallOptions options)
      {
        return CallInvoker.AsyncClientStreamingCall(__Method_HealthCheck, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override DataNodeHealthCenterClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new DataNodeHealthCenterClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(DataNodeHealthCenterBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_HealthCheck, serviceImpl.HealthCheck).Build();
    }

  }
  public static partial class Pipelining
  {
    static readonly string __ServiceName = "sufs.Pipelining";

    static readonly grpc::Marshaller<global::Sufs.ReplicateRequest> __Marshaller_ReplicateRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.ReplicateRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.ReplicateResponse> __Marshaller_ReplicateResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.ReplicateResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Sufs.ReplicateRequest, global::Sufs.ReplicateResponse> __Method_ReplicateBlock = new grpc::Method<global::Sufs.ReplicateRequest, global::Sufs.ReplicateResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ReplicateBlock",
        __Marshaller_ReplicateRequest,
        __Marshaller_ReplicateResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Sufs.DataNodeReflection.Descriptor.Services[2]; }
    }

    /// <summary>Base class for server-side implementations of Pipelining</summary>
    public abstract partial class PipeliningBase
    {
      public virtual global::System.Threading.Tasks.Task<global::Sufs.ReplicateResponse> ReplicateBlock(global::Sufs.ReplicateRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for Pipelining</summary>
    public partial class PipeliningClient : grpc::ClientBase<PipeliningClient>
    {
      /// <summary>Creates a new client for Pipelining</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public PipeliningClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for Pipelining that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public PipeliningClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected PipeliningClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected PipeliningClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual global::Sufs.ReplicateResponse ReplicateBlock(global::Sufs.ReplicateRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return ReplicateBlock(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sufs.ReplicateResponse ReplicateBlock(global::Sufs.ReplicateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_ReplicateBlock, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.ReplicateResponse> ReplicateBlockAsync(global::Sufs.ReplicateRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return ReplicateBlockAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.ReplicateResponse> ReplicateBlockAsync(global::Sufs.ReplicateRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_ReplicateBlock, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override PipeliningClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new PipeliningClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(PipeliningBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_ReplicateBlock, serviceImpl.ReplicateBlock).Build();
    }

  }
}
#endregion
