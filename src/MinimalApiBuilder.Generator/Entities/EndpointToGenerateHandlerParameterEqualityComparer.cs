﻿namespace MinimalApiBuilder.Generator.Entities;

internal class EndpointToGenerateHandlerParameterEqualityComparer :
    IEqualityComparer<EndpointToGenerateHandlerParameter>
{
    public static readonly EndpointToGenerateHandlerParameterEqualityComparer Instance = new();

    public bool Equals(EndpointToGenerateHandlerParameter? x, EndpointToGenerateHandlerParameter? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        return x.Position == y.Position;
    }

    public int GetHashCode(EndpointToGenerateHandlerParameter obj)
    {
        throw new NotImplementedException();
    }
}
