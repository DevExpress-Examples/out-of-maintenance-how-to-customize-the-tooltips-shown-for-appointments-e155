using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CustomAppointmentEditForm {
    public partial class Form1 : Form {
        const string aptDataFileName = @"..\..\Data\appointments.xml";
        const string resDataFileName = @"..\..\Data\resources.xml";
        Image resImage;
        
        public Form1() {
            InitializeComponent();
            schedulerControl1.ToolTipController = toolTipController1;
            FillData();

            this.schedulerDataStorage1.AppointmentsChanged += schedulerDataStorage_AppointmentsChanged;
            this.schedulerDataStorage1.AppointmentsInserted += schedulerDataStorage_AppointmentsChanged;
            this.schedulerDataStorage1.AppointmentsDeleted += schedulerDataStorage_AppointmentsChanged;

            this.schedulerControl1.AppointmentViewInfoCustomizing += SchedulerControl1_AppointmentViewInfoCustomizing;

            resImage = Image.FromFile(@"..\..\Images\appointment.gif");
            toolTipController1.ShowBeak = true;

            schedulerControl1.Start = new DateTime(2010, 7, 11);
            schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Always;
            toolTipController1.ToolTipType = ToolTipType.Standard;            

            schedulerControl1.OptionsCustomization.AllowDisplayAppointmentFlyout = false;
        }

        #region #tooltip_EmptySubject
        private void SchedulerControl1_AppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e) {
            if (e.ViewInfo.DisplayText == String.Empty)
                e.ViewInfo.ToolTipText = String.Format("Started at {0:g}", e.ViewInfo.Appointment.Start);
        }
        #endregion #tooltip_EmptySubject

        #region #ToolTipControllerBeforeShow
        private void toolTipController1_BeforeShow(object sender, ToolTipControllerShowEventArgs e) {
            ToolTipController controller = sender as ToolTipController;
            AppointmentViewInfo aptViewInfo = controller.ActiveObject as AppointmentViewInfo;
            if (aptViewInfo == null) return;

            if (toolTipController1.ToolTipType == ToolTipType.Standard) {
                e.IconType = ToolTipIconType.Information;
                e.ToolTip = aptViewInfo.Description;
            }

            if (toolTipController1.ToolTipType == ToolTipType.SuperTip) {
                SuperToolTip SuperTip = new SuperToolTip();
                SuperToolTipSetupArgs args = new SuperToolTipSetupArgs();
                args.Title.Text = "Info";
                args.Title.Font = new Font("Times New Roman", 14);
                args.Contents.Text = aptViewInfo.Description;
                args.Contents.Image = resImage;
                args.ShowFooterSeparator = true;
                args.Footer.Font = new Font("Comic Sans MS", 8);
                args.Footer.Text = "SuperTip";
                SuperTip.Setup(args);
                e.SuperTip = SuperTip;
            }
        }
        #endregion #ToolTipControllerBeforeShow

        #region FillData
        void FillData() {
            AppointmentCustomFieldMapping customNameMapping = new AppointmentCustomFieldMapping("CustomName", "CustomName");
            AppointmentCustomFieldMapping customStatusMapping = new AppointmentCustomFieldMapping("CustomStatus", "CustomStatus");
            schedulerDataStorage1.Appointments.CustomFieldMappings.Add(customNameMapping);
            schedulerDataStorage1.Appointments.CustomFieldMappings.Add(customStatusMapping);
            FillResourcesStorage(schedulerDataStorage1.Resources.Items, resDataFileName);
            FillAppointmentsStorage(schedulerDataStorage1.Appointments.Items, aptDataFileName);
        }

        static Stream GetFileStream(string fileName) {
            return new StreamReader(fileName).BaseStream;
        }

        static void FillAppointmentsStorage(AppointmentCollection c, string fileName) {
            using (Stream stream = GetFileStream(fileName)) {
                c.ReadXml(stream);
                stream.Close();
            }
        }

        static void FillResourcesStorage(ResourceCollection c, string fileName) {
            using (Stream stream = GetFileStream(fileName)) {
                c.ReadXml(stream);
                stream.Close();
            }
        }
        #endregion

        private void schedulerDataStorage_AppointmentsChanged(object sender, PersistentObjectsEventArgs e) {
            schedulerDataStorage1.Appointments.Items.WriteXml(aptDataFileName);
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e) {
            if (this.toggleSwitch1.IsOn) {
                this.schedulerControl1.AppointmentViewInfoCustomizing -= SchedulerControl1_AppointmentViewInfoCustomizing;
                toolTipController1.BeforeShow += toolTipController1_BeforeShow;
                checkEdit1.Visible = true;
            }
            else {
                this.schedulerControl1.AppointmentViewInfoCustomizing += SchedulerControl1_AppointmentViewInfoCustomizing;
                this.toolTipController1.BeforeShow -= toolTipController1_BeforeShow;
                checkEdit1.Visible = false;
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e) {
            toolTipController1.ToolTipType = (checkEdit1.Checked) ? ToolTipType.SuperTip : ToolTipType.Standard;
        }
    }
}