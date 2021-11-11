#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Collections;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509
{
	/// <remarks>Generator for X.509 extensions</remarks>
	public class X509ExtensionsGenerator
	{
		private IDictionary extensions = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateHashtable();
        private IList extOrdering = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList();

		/// <summary>Reset the generator</summary>
		public void Reset()
		{
            extensions = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateHashtable();
            extOrdering = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList();
		}

		/// <summary>
		/// Add an extension with the given oid and the passed in value to be included
		/// in the OCTET STRING associated with the extension.
		/// </summary>
		/// <param name="oid">OID for the extension.</param>
		/// <param name="critical">True if critical, false otherwise.</param>
		/// <param name="extValue">The ASN.1 object to be included in the extension.</param>
		public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			Asn1Encodable		extValue)
		{
			byte[] encoded;
			try
			{
				encoded = extValue.GetDerEncoded();
			}
			catch (Exception e)
			{
				throw new ArgumentException("error encoding value: " + e);
			}

			this.AddExtension(oid, critical, encoded);
		}

		/// <summary>
		/// Add an extension with the given oid and the passed in byte array to be wrapped
		/// in the OCTET STRING associated with the extension.
		/// </summary>
		/// <param name="oid">OID for the extension.</param>
		/// <param name="critical">True if critical, false otherwise.</param>
		/// <param name="extValue">The byte array to be wrapped.</param>
		public void AddExtension(
			DerObjectIdentifier	oid,
			bool				critical,
			byte[]				extValue)
		{
			if (extensions.Contains(oid))
			{
				throw new ArgumentException("extension " + oid + " already added");
			}

			extOrdering.Add(oid);
			extensions.Add(oid, new X509Extension(critical, new DerOctetString(extValue)));
		}

		/// <summary>Return true if there are no extension present in this generator.</summary>
		/// <returns>True if empty, false otherwise</returns>
		public bool IsEmpty
		{
			get { return extOrdering.Count < 1; }
		}

		/// <summary>Generate an X509Extensions object based on the current state of the generator.</summary>
		/// <returns>An <c>X509Extensions</c> object</returns>
		public X509Extensions Generate()
		{
			return new X509Extensions(extOrdering, extensions);
		}
	}
}
#pragma warning restore
#endif
