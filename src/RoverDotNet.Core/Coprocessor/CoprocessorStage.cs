namespace RoverDotNet.Core.Coprocessor;

/// <summary>
/// The router request-handling lifecycle stage a coprocessor payload relates to.
/// Mirrors the <c>stage</c> property of the Apollo Router coprocessor JSON contract.
/// </summary>
/// <remarks>
/// See: https://www.apollographql.com/docs/graphos/routing/customization/coprocessor
/// </remarks>
public enum CoprocessorStage
{
    /// <summary>The router has just received a client request.</summary>
    RouterRequest,

    /// <summary>The router is about to send a response to the client.</summary>
    RouterResponse,

    /// <summary>The supergraph service has just received a client request.</summary>
    SupergraphRequest,

    /// <summary>The supergraph service is about to send a response to the client.</summary>
    SupergraphResponse,

    /// <summary>The router is about to send a request to a subgraph.</summary>
    SubgraphRequest,

    /// <summary>The router has just received a response from a subgraph.</summary>
    SubgraphResponse,
}
