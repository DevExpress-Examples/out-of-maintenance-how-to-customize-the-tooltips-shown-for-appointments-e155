Imports DevExpress.Utils
Imports DevExpress.XtraScheduler
Imports DevExpress.XtraScheduler.Drawing
Imports System
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms

Namespace CustomAppointmentEditForm
    Partial Public Class Form1
        Inherits Form

        Private Const aptDataFileName As String = "..\..\Data\appointments.xml"
        Private Const resDataFileName As String = "..\..\Data\resources.xml"
        Private resImage As Image

        Public Sub New()
            InitializeComponent()
            schedulerControl1.ToolTipController = toolTipController1
            FillData()

            AddHandler Me.schedulerDataStorage1.AppointmentsChanged, AddressOf schedulerDataStorage_AppointmentsChanged
            AddHandler Me.schedulerDataStorage1.AppointmentsInserted, AddressOf schedulerDataStorage_AppointmentsChanged
            AddHandler Me.schedulerDataStorage1.AppointmentsDeleted, AddressOf schedulerDataStorage_AppointmentsChanged

            AddHandler Me.schedulerControl1.AppointmentViewInfoCustomizing, AddressOf SchedulerControl1_AppointmentViewInfoCustomizing

            resImage = Image.FromFile("..\..\Images\appointment.gif")
            toolTipController1.ShowBeak = True

            schedulerControl1.Start = New Date(2010, 7, 11)
            schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Always
            toolTipController1.ToolTipType = ToolTipType.Standard

            schedulerControl1.OptionsCustomization.AllowDisplayAppointmentFlyout = False
        End Sub

        #Region "#tooltip_EmptySubject"
        Private Sub SchedulerControl1_AppointmentViewInfoCustomizing(ByVal sender As Object, ByVal e As AppointmentViewInfoCustomizingEventArgs)
            If e.ViewInfo.DisplayText = String.Empty Then
                e.ViewInfo.ToolTipText = String.Format("Started at {0:g}", e.ViewInfo.Appointment.Start)
            End If
        End Sub
        #End Region ' #tooltip_EmptySubject

        #Region "#ToolTipControllerBeforeShow"
        Private Sub toolTipController1_BeforeShow(ByVal sender As Object, ByVal e As ToolTipControllerShowEventArgs)
            Dim controller As ToolTipController = TryCast(sender, ToolTipController)
            Dim aptViewInfo As AppointmentViewInfo = TryCast(controller.ActiveObject, AppointmentViewInfo)
            If aptViewInfo Is Nothing Then
                Return
            End If

            If toolTipController1.ToolTipType = ToolTipType.Standard Then
                e.IconType = ToolTipIconType.Information
                e.ToolTip = aptViewInfo.Description
            End If

            If toolTipController1.ToolTipType = ToolTipType.SuperTip Then
                Dim SuperTip As New SuperToolTip()
                Dim args As New SuperToolTipSetupArgs()
                args.Title.Text = "Info"
                args.Title.Font = New Font("Times New Roman", 14)
                args.Contents.Text = aptViewInfo.Description
                args.Contents.Image = resImage
                args.ShowFooterSeparator = True
                args.Footer.Font = New Font("Comic Sans MS", 8)
                args.Footer.Text = "SuperTip"
                SuperTip.Setup(args)
                e.SuperTip = SuperTip
            End If
        End Sub
        #End Region ' #ToolTipControllerBeforeShow

        #Region "FillData"
        Private Sub FillData()
            Dim customNameMapping As New AppointmentCustomFieldMapping("CustomName", "CustomName")
            Dim customStatusMapping As New AppointmentCustomFieldMapping("CustomStatus", "CustomStatus")
            schedulerDataStorage1.Appointments.CustomFieldMappings.Add(customNameMapping)
            schedulerDataStorage1.Appointments.CustomFieldMappings.Add(customStatusMapping)
            FillResourcesStorage(schedulerDataStorage1.Resources.Items, resDataFileName)
            FillAppointmentsStorage(schedulerDataStorage1.Appointments.Items, aptDataFileName)
        End Sub

        Private Shared Function GetFileStream(ByVal fileName As String) As Stream
            Return (New StreamReader(fileName)).BaseStream
        End Function

        Private Shared Sub FillAppointmentsStorage(ByVal c As AppointmentCollection, ByVal fileName As String)
            Using stream As Stream = GetFileStream(fileName)
                c.ReadXml(stream)
                stream.Close()
            End Using
        End Sub

        Private Shared Sub FillResourcesStorage(ByVal c As ResourceCollection, ByVal fileName As String)
            Using stream As Stream = GetFileStream(fileName)
                c.ReadXml(stream)
                stream.Close()
            End Using
        End Sub
        #End Region

        Private Sub schedulerDataStorage_AppointmentsChanged(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
            schedulerDataStorage1.Appointments.Items.WriteXml(aptDataFileName)
        End Sub

        Private Sub toggleSwitch1_Toggled(ByVal sender As Object, ByVal e As EventArgs) Handles toggleSwitch1.Toggled
            If Me.toggleSwitch1.IsOn Then
                RemoveHandler Me.schedulerControl1.AppointmentViewInfoCustomizing, AddressOf SchedulerControl1_AppointmentViewInfoCustomizing
                AddHandler toolTipController1.BeforeShow, AddressOf toolTipController1_BeforeShow
                checkEdit1.Visible = True
            Else
                AddHandler Me.schedulerControl1.AppointmentViewInfoCustomizing, AddressOf SchedulerControl1_AppointmentViewInfoCustomizing
                RemoveHandler Me.toolTipController1.BeforeShow, AddressOf toolTipController1_BeforeShow
                checkEdit1.Visible = False
            End If
        End Sub

        Private Sub checkEdit1_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles checkEdit1.CheckedChanged
            toolTipController1.ToolTipType = If(checkEdit1.Checked, ToolTipType.SuperTip, ToolTipType.Standard)
        End Sub
    End Class
End Namespace