using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestTPEGBinary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogBrowse = new OpenFileDialog();
            DialogResult result = dialogBrowse.ShowDialog();
            if (result == DialogResult.OK) 
            {
                string responseGZip = File.ReadAllText(dialogBrowse.FileName);
                byte[] parsedValue = ParseResponseContentAsbyteAsync(Convert.FromBase64String(responseGZip)).Result;
            }
        }

        private async Task<byte[]> ParseResponseContentAsbyteAsync(byte[] responseGZipContent)
        {
            MemoryStream responseStream = new MemoryStream();
            byte[] tpegFrame;
            try
            {
                using (MemoryStream responseGzipStream = new MemoryStream(responseGZipContent))
                {
                    using (GZipStream responseDecompressionStream = new GZipStream(responseGzipStream, CompressionMode.Decompress))
                        await responseDecompressionStream.CopyToAsync(responseStream).ConfigureAwait(false);

                    tpegFrame = responseStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while parsing the response data from TDP", ex);
            }
            return tpegFrame;
        }
    }
}
