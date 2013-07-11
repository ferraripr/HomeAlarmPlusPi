using System    ;
using Microsoft.SPOT;

namespace Bansky.SPOT.Mail
{
    /// <summary>
    /// Specifies the Content-Transfer-Encoding header information for an e-mail message attachment.
    /// </summary>
    public enum TransferEncoding
    {
        /// <summary>
        /// Encodes data that consists of printable characters in the US-ASCII character set. See RFC 2406 Section 6.7.
        /// </summary>
        QuotedPrintable,
        /// <summary>
        /// Encodes stream-based data. See RFC 2406 Section 6.8.
        /// </summary>
	    Base64,
        /// <summary>
        /// Used for data that is not encoded. The data is in 7-bit US-ASCII characters with a total line length of no longer than 1000 characters. See RFC2406 Section 2.7.
        /// </summary>
	    SevenBit,
        /// <summary>
        /// Indicates that the transfer encoding is unknown.
        /// </summary>
	    Unknown
    }
}
