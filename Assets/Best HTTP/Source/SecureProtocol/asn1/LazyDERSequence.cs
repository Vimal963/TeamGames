#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System.Collections;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1
{
	internal class LazyDerSequence
		: DerSequence
	{
		private byte[] encoded;

        internal LazyDerSequence(
			byte[] encoded)
		{
			this.encoded = encoded;
		}

		private void Parse()
		{
			lock (this)
			{
                if (null != encoded)
                {
                    Asn1EncodableVector v = new Asn1EncodableVector();
                    Asn1InputStream e = new LazyAsn1InputStream(encoded);

                    Asn1Object o;
                    while ((o = e.ReadObject()) != null)
                    {
                        v.Add(o);
                    }

                    this.elements = v.TakeElements();
                    this.encoded = null;
                }
			}
		}

		public override Asn1Encodable this[int index]
		{
			get
			{
				Parse();

				return base[index];
			}
		}

		public override IEnumerator GetEnumerator()
		{
			Parse();

			return base.GetEnumerator();
		}

		public override int Count
		{
			get
			{
				Parse();

				return base.Count;
			}
		}

		internal override void Encode(
			DerOutputStream derOut)
		{
			lock (this)
			{
				if (encoded == null)
				{
					base.Encode(derOut);
				}
				else
				{
					derOut.WriteEncoded(Asn1Tags.Sequence | Asn1Tags.Constructed, encoded);
				}
			}
		}
	}
}
#pragma warning restore
#endif
