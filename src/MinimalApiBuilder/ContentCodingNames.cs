namespace MinimalApiBuilder;

/// <summary>
/// Defines constants of the
/// <a href="https://www.iana.org/assignments/http-parameters/http-parameters.xhtml#content-coding">HTTP Content Coding Registry</a>.
/// </summary>
public static class ContentCodingNames
{
    /// <summary>
    /// AES-GCM encryption with a 128-bit content encryption key.
    /// </summary>
    public const string Aes128Gcm = "aes128gcm";

    /// <summary>
    /// Brotli Compressed Data Format.
    /// </summary>
    public const string Br = "br";

    /// <summary>
    /// UNIX "compress" data format.
    /// </summary>
    public const string Compress = "compress";

    /// <summary>
    /// "deflate" compressed data inside the "zlib" data format.
    /// </summary>
    public const string Deflate = "deflate";

    /// <summary>
    /// W3C Efficient XML Interchange.
    /// </summary>
    public const string Exi = "exi";

    /// <summary>
    /// GZIP file format.
    /// </summary>
    public const string Gzip = "gzip";

    /// <summary>
    /// Reserved.
    /// </summary>
    public const string Identity = "identity";

    /// <summary>
    /// Network Transfer Format for Java Archives.
    /// </summary>
    public const string Pack200Gzip = "pack200-gzip";

    /// <summary>
    /// Deprecated (alias for compress).
    /// </summary>
    public const string XCompress = "x-compress";

    /// <summary>
    /// Deprecated (alias for gzip).
    /// </summary>
    public const string XGzip = "x-gzip";

    /// <summary>
    /// A stream of bytes compressed using the Zstandard protocol.
    /// </summary>
    public const string Zstd = "zstd";
}
