using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

using Microsoft.Win32;

namespace License
{
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label LicenseLabel;
		private System.Windows.Forms.Button SignButton;
		private System.Windows.Forms.Button BrowseButton;
		private System.Windows.Forms.TextBox LicenseTextBox;
		private System.Windows.Forms.Button VerifyButton;
		private System.Windows.Forms.OpenFileDialog OpenDialog;
		private System.Windows.Forms.SaveFileDialog SaveDialog;
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LicenseTextBox = new System.Windows.Forms.TextBox();
			this.LicenseLabel = new System.Windows.Forms.Label();
			this.SignButton = new System.Windows.Forms.Button();
			this.BrowseButton = new System.Windows.Forms.Button();
			this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
			this.VerifyButton = new System.Windows.Forms.Button();
			this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// LicenseTextBox
			// 
			this.LicenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.LicenseTextBox.Location = new System.Drawing.Point(64, 8);
			this.LicenseTextBox.Name = "LicenseTextBox";
			this.LicenseTextBox.Size = new System.Drawing.Size(224, 20);
			this.LicenseTextBox.TabIndex = 0;
			this.LicenseTextBox.Text = "";
			// 
			// LicenseLabel
			// 
			this.LicenseLabel.Location = new System.Drawing.Point(8, 8);
			this.LicenseLabel.Name = "LicenseLabel";
			this.LicenseLabel.Size = new System.Drawing.Size(56, 24);
			this.LicenseLabel.TabIndex = 1;
			this.LicenseLabel.Text = "License";
			this.LicenseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SignButton
			// 
			this.SignButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.SignButton.Location = new System.Drawing.Point(200, 40);
			this.SignButton.Name = "SignButton";
			this.SignButton.Size = new System.Drawing.Size(88, 23);
			this.SignButton.TabIndex = 2;
			this.SignButton.Text = "Sign License";
			this.SignButton.Click += new System.EventHandler(this.SignLicense);
			// 
			// BrowseButton
			// 
			this.BrowseButton.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.BrowseButton.Location = new System.Drawing.Point(296, 8);
			this.BrowseButton.Name = "BrowseButton";
			this.BrowseButton.TabIndex = 3;
			this.BrowseButton.Text = "Browse";
			this.BrowseButton.Click += new System.EventHandler(this.BrowseForLicense);
			// 
			// OpenDialog
			// 
			this.OpenDialog.DefaultExt = "xml";
			this.OpenDialog.FileName = "License.xml";
			// 
			// VerifyButton
			// 
			this.VerifyButton.Location = new System.Drawing.Point(296, 40);
			this.VerifyButton.Name = "VerifyButton";
			this.VerifyButton.TabIndex = 4;
			this.VerifyButton.Text = "Verify";
			this.VerifyButton.Click += new System.EventHandler(this.Verify);
			// 
			// SaveDialog
			// 
			this.SaveDialog.FileName = "license.xml";
			// 
			// MainForm
			// 
			this.AcceptButton = this.SignButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 69);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.VerifyButton,
																		  this.BrowseButton,
																		  this.SignButton,
																		  this.LicenseLabel,
																		  this.LicenseTextBox});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.Text = "Object Server License Manager";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void BrowseForLicense(object sender, System.EventArgs e)
		{
			if(OpenDialog.ShowDialog(this) != DialogResult.OK)
				return;

			LicenseTextBox.Text = OpenDialog.FileName;
		}

		private void SignLicense(object sender, System.EventArgs e)
		{
			if (!File.Exists(LicenseTextBox.Text))
			{
				return;
			}

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(LicenseTextBox.Text);

			CspParameters parms = new CspParameters(1);
			parms.Flags = CspProviderFlags.UseMachineKeyStore;
			parms.KeyContainerName = "ObjectServerLicense";
			parms.KeyNumber = 2;
			RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);

			SignedXml sxml = new SignedXml(xmldoc);
			sxml.SigningKey = csp;

			sxml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;

			Reference r = new Reference("");

			r.AddTransform(new XmlDsigEnvelopedSignatureTransform(false));

			sxml.AddReference(r);

			sxml.ComputeSignature();

			XmlElement sig = sxml.GetXml();
			xmldoc.DocumentElement.AppendChild(sig);

			if(SaveDialog.ShowDialog(this) != DialogResult.OK)
				return;

			string filename = SaveDialog.FileName;

			XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			try
			{
				xmldoc.WriteTo(writer);
				MessageBox.Show(this, "Licence created");;
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, "Licence creation failed " + ex.Message);
			}
			finally
			{
				writer.Flush();
				writer.Close();
			}
		}

		private void Verify(object sender, System.EventArgs e)
		{
			Stream s = null;
			string xmlkey = string.Empty;
			try
			{
				s = typeof(MainForm).Assembly.GetManifestResourceStream("License.PublicKey.xml");

				StreamReader reader = new StreamReader(s);
				xmlkey = reader.ReadToEnd();
				reader.Close();
			}
			catch
			{
				throw new Exception("Error: could not import public key");
			}

			CspParameters parms = new CspParameters();
			parms.Flags = CspProviderFlags.UseMachineKeyStore;
			RSACryptoServiceProvider csp = new RSACryptoServiceProvider(parms);
			csp.FromXmlString(xmlkey);

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(LicenseTextBox.Text);

			SignedXml sxml = new SignedXml(xmldoc);

			try
			{
				XmlNode dsig = xmldoc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
				sxml.LoadXml((XmlElement)dsig);
			}
			catch
			{
				throw new Exception("Error: could not import public key");
			}

			if (sxml.CheckSignature(csp))
				MessageBox.Show(this, "Success");
			else
				MessageBox.Show(this, "Failure");
		}
	}
}
