using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace GreenDonut
{
    internal static class CancellationTokenSourceExtensions
    {
        public static CancellationToken CreateLinkedCancellationToken(
            [DisallowNull]this CancellationTokenSource? source,
            CancellationToken cancellationToken)
        {
            if (source == null)
            {
                return cancellationToken;
            }

            if (cancellationToken == CancellationToken.None)
            {
                return source.Token;
            }

            return CancellationTokenSource.CreateLinkedTokenSource(
                source.Token,
                cancellationToken)
                    .Token;
        }
    }
}
