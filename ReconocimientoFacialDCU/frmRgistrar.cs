using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace ReconocimientoFacialDCU
{
    public partial class frmRgistrar : Form
    {

        // Variables vectores y Harcascades
        int con = 0;
        Image<Bgr, Byte> currentFrame;
        Capture Grabar;
        HaarCascade face;//Rostro
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.8d, 0.8d);
        Image<Gray, byte> result, TraineFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NombrePersonas = new List<string>();
        List<string> TelefonoPersona = new List<string>();
        List<string> CorreoPersona = new List<string>();
        int ContTrain, numLabels, t;
        string Nombre;
        string Telefono;
        string Correo;
        DataGridView d = new DataGridView();

        public frmRgistrar()
        {
            InitializeComponent();

            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                clsDA.Consultar(d);

                string[] Labels = clsDA.Nombre;
                string[] Labels1 = clsDA.Telefono;
                string[] Labels2 = clsDA.Correo;
                numLabels = clsDA.TotalRostros;
                ContTrain = numLabels;

                for (int i = 0; i < numLabels; i++)
                {
                    con = i;
                    Bitmap bmp = new Bitmap(clsDA.ConvertBinaryToImg(con));

                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[i]);
                    labels.Add(Labels1[i]);
                    labels.Add(Labels2[i]);

                }



            }
            catch (Exception e)
            {

                MessageBox.Show("Error" + e);
            }
        }

        private void FrameGrabar(object sender, EventArgs e)
        {
            lblCantidad.Text = "0";
            NombrePersonas.Add("");
            TelefonoPersona.Add("");
            CorreoPersona.Add("");

            try
            {
                currentFrame = Grabar.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);

                gray = currentFrame.Convert<Gray, Byte>();

                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(5, 5));

                foreach (MCvAvgComp R in RostrosDetectados[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    currentFrame.Draw(R.rect, new Bgr(Color.Green), 2);



                    NombrePersonas[t - 1] = Nombre;
                    NombrePersonas.Add("");

                    TelefonoPersona[t - 1] = Telefono;
                    TelefonoPersona.Add("");

                    CorreoPersona[t - 1] = Correo;
                    CorreoPersona.Add("");

                    lblCantidad.Text = RostrosDetectados[0].Length.ToString();
                }
                t = 0;
                imageBox1.Image = currentFrame;
                Nombre = "";
                Telefono = "";
                Correo = "";
                NombrePersonas.Clear();
                TelefonoPersona.Clear();
                CorreoPersona.Clear();
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }

        }

        private void frmRgistrar_Load(object sender, EventArgs e)
        {
            reconocer();
            clsDA.Consultar(dataGridView1);
        }

        private void frmRgistrar_FormClosing(object sender, FormClosingEventArgs e)
        {
            DetenerReconocer();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (txtNombre.Text != "")
            {
                labels.Add(txtNombre.Text);
                clsDA.GuardarImagen(txtNombre.Text, txtTelefono.Text, txtCorreo.Text, clsDA.ConvertImgToBinary(imageBox2.Image.Bitmap));


            }

            clsDA.Consultar(dataGridView1);
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnCapturar_Click(object sender, EventArgs e)
        {
            try
            {
                ContTrain += ContTrain;
                gray = currentFrame.Convert<Gray, Byte>();

                MCvAvgComp[][] RostrosDetectados = gray.DetectHaarCascade(face, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                foreach (MCvAvgComp R in RostrosDetectados[0])
                {

                    TraineFace = currentFrame.Copy(R.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    break;

                }
                TraineFace = result.Resize(100, 100, INTER.CV_INTER_CUBIC);
                trainingImages.Add(TraineFace);

                imageBox2.Image = TraineFace;
            }
            catch
            {


            }
        }

        private void reconocer()
        {
            try
            {
                Grabar = new Capture();
                Grabar.QueryFrame();
                Application.Idle += new EventHandler(FrameGrabar);
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }
        }

        private void DetenerReconocer()
        {
            try
            {

                Application.Idle -= new EventHandler(FrameGrabar);
                Grabar.Dispose();
            }
            catch (Exception Error)
            {

                MessageBox.Show(Error.Message);
            }

        }


    }
}
