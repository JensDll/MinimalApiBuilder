namespace Web.Features.Customer.List;

public struct Parameters
{
    public Parameters(int offset = 1, int limit = 10)
    {
        Offset = offset < 1 ? 1 : offset;
        Limit = limit < 1 ? 10 : limit;
    }

    public int Offset { get; }

    public int Limit { get; }
}
