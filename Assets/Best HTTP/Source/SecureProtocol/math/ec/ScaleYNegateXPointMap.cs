#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC
{
    public class ScaleYNegateXPointMap
        : ECPointMap
    {
        protected readonly ECFieldElement scale;

        public ScaleYNegateXPointMap(ECFieldElement scale)
        {
            this.scale = scale;
        }

        public virtual ECPoint Map(ECPoint p)
        {
            return p.ScaleYNegateX(scale);
        }
    }
}
#pragma warning restore
#endif