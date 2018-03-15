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
  public static partial class DataNode
  {
    static readonly string __ServiceName = "sufs.DataNode";

    static readonly grpc::Marshaller<global::Sufs.WriteRequest> __Marshaller_WriteRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.WriteRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.WriteResponse> __Marshaller_WriteResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.WriteResponse.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.ReadBlockRequest> __Marshaller_ReadBlockRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.ReadBlockRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.ReadBlockResponse> __Marshaller_ReadBlockResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.ReadBlockResponse.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.DeleteRequest> __Marshaller_DeleteRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.DeleteRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.DeleteResponse> __Marshaller_DeleteResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.DeleteResponse.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.HealthRequest> __Marshaller_HealthRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.HealthRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Sufs.HealthResponse> __Marshaller_HealthResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Sufs.HealthResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse> __Method_WriteFile = new grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "WriteFile",
        __Marshaller_WriteRequest,
        __Marshaller_WriteResponse);

    static readonly grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse> __Method_ReplicateBlock = new grpc::Method<global::Sufs.WriteRequest, global::Sufs.WriteResponse>(
        grpc::MethodType.ClientStreaming,
        __ServiceName,
        "ReplicateBlock",
        __Marshaller_WriteRequest,
        __Marshaller_WriteResponse);

    static readonly grpc::Method<global::Sufs.ReadBlockRequest, global::Sufs.ReadBlockResponse> __Method_ReadFile = new grpc::Method<global::Sufs.ReadBlockRequest, global::Sufs.ReadBlockResponse>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "ReadFile",
        __Marshaller_ReadBlockRequest,
        __Marshaller_ReadBlockResponse);

    static readonly grpc::Method<global::Sufs.DeleteRequest, global::Sufs.DeleteResponse> __Method_DeleteFile = new grpc::Method<global::Sufs.DeleteRequest, global::Sufs.DeleteResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "DeleteFile",
        __Marshaller_DeleteRequest,
        __Marshaller_DeleteResponse);

    static readonly grpc::Method<global::Sufs.HealthRequest, global::Sufs.HealthResponse> __Method_HealthCheck = new grpc::Method<global::Sufs.HealthRequest, global::Sufs.HealthResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "HealthCheck",
        __Marshaller_HealthRequest,
        __Marshaller_HealthResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Sufs.DataNodeReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of DataNode</summary>
    public abstract partial class DataNodeBase
    {
      public virtual global::System.Threading.Tasks.Task<global::Sufs.WriteResponse> WriteFile(grpc::IAsyncStreamReader<global::Sufs.WriteRequest> requestStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::Sufs.WriteResponse> ReplicateBlock(grpc::IAsyncStreamReader<global::Sufs.WriteRequest> requestStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task ReadFile(global::Sufs.ReadBlockRequest request, grpc::IServerStreamWriter<global::Sufs.ReadBlockResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::Sufs.DeleteResponse> DeleteFile(global::Sufs.DeleteRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      public virtual global::System.Threading.Tasks.Task<global::Sufs.HealthResponse> HealthCheck(global::Sufs.HealthRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for DataNode</summary>
    public partial class DataNodeClient : grpc::ClientBase<DataNodeClient>
    {
      /// <summary>Creates a new client for DataNode</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public DataNodeClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for DataNode that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public DataNodeClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected DataNodeClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected DataNodeClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      public virtual grpc::AsyncClientStreamingCall<global::Sufs.WriteRequest, global::Sufs.WriteResponse> WriteFile(grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return WriteFile(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncClientStreamingCall<global::Sufs.WriteRequest, global::Sufs.WriteResponse> WriteFile(grpc::CallOptions options)
      {
        return CallInvoker.AsyncClientStreamingCall(__Method_WriteFile, null, options);
      }
      public virtual grpc::AsyncClientStreamingCall<global::Sufs.WriteRequest, global::Sufs.WriteResponse> ReplicateBlock(grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return ReplicateBlock(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncClientStreamingCall<global::Sufs.WriteRequest, global::Sufs.WriteResponse> ReplicateBlock(grpc::CallOptions options)
      {
        return CallInvoker.AsyncClientStreamingCall(__Method_ReplicateBlock, null, options);
      }
      public virtual grpc::AsyncServerStreamingCall<global::Sufs.ReadBlockResponse> ReadFile(global::Sufs.ReadBlockRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return ReadFile(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncServerStreamingCall<global::Sufs.ReadBlockResponse> ReadFile(global::Sufs.ReadBlockRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncServerStreamingCall(__Method_ReadFile, null, options, request);
      }
      public virtual global::Sufs.DeleteResponse DeleteFile(global::Sufs.DeleteRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return DeleteFile(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sufs.DeleteResponse DeleteFile(global::Sufs.DeleteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_DeleteFile, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.DeleteResponse> DeleteFileAsync(global::Sufs.DeleteRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return DeleteFileAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.DeleteResponse> DeleteFileAsync(global::Sufs.DeleteRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_DeleteFile, null, options, request);
      }
      public virtual global::Sufs.HealthResponse HealthCheck(global::Sufs.HealthRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return HealthCheck(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual global::Sufs.HealthResponse HealthCheck(global::Sufs.HealthRequest request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_HealthCheck, null, options, request);
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.HealthResponse> HealthCheckAsync(global::Sufs.HealthRequest request, grpc::Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        return HealthCheckAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      public virtual grpc::AsyncUnaryCall<global::Sufs.HealthResponse> HealthCheckAsync(global::Sufs.HealthRequest request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_HealthCheck, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override DataNodeClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new DataNodeClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(DataNodeBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_WriteFile, serviceImpl.WriteFile)
          .AddMethod(__Method_ReplicateBlock, serviceImpl.ReplicateBlock)
          .AddMethod(__Method_ReadFile, serviceImpl.ReadFile)
          .AddMethod(__Method_DeleteFile, serviceImpl.DeleteFile)
          .AddMethod(__Method_HealthCheck, serviceImpl.HealthCheck).Build();
    }

  }
}
#endregion