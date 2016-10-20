﻿Imports System.IO
Imports System.Text.Encoding
Imports System.Threading
Imports System.Management
Imports System.Runtime.Serialization.Json

Public Class Form1

    Private Property exec_path As String = Nothing
    Private Property argv As String = Nothing
    Private Property proxy_set As Boolean = False

    Private configdir As String = Application.LocalUserAppDataPath + "\client.json"

    Dim Process1 As Process = Nothing
    Dim Process2 As Process = Nothing

    Private Sub Clean()
        RichTextBox1.Text = ""
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Clean()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Clean()
        If Me.exec_path Is Nothing Then
            MsgBox("Cannot run ArkC. " + vbNewLine + "No valid configuration loaded.", vbExclamation, "Error")
        Else
            Execute()
        End If
    End Sub

    Private Sub Execute()
        If Process1 IsNot Nothing Then Process1.Dispose()
        Process1 = New Process
        AddHandler Process1.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
        AddHandler Process1.Exited, AddressOf Process1_Exited
        With Process1
            .EnableRaisingEvents = True
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.CreateNoWindow = True
            .StartInfo.FileName = Me.exec_path
            .StartInfo.Arguments = " -v -c """ & configdir & """ " & Me.argv
            .Start()
            ToolStripStatusLabel1.Text = "Running with config: " + configdir
            ToolStripStatusLabel2.Text = "Running"
            Button1.Enabled = False
        End With
        Dim runThread = New Thread(AddressOf Process1_starting)
        runThread.Start()
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            If Not (Process1.HasExited) Then killtree(Process1.Id)
        Catch

        End Try
        Try
            ProxyConfig.DisableProxy()
            Me.proxy_set = False
        Catch

        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Form1.CheckForIllegalCrossThreadCalls = False
    End Sub


    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            Check_config()
        Catch

        End Try
    End Sub

    Private Sub Check_config()
        If System.IO.File.Exists(configdir) Then
            Load_config()
        Else
            show_form2()
        End If

    End Sub

    Public Sub Load_config()
        Dim fsTemp As New System.IO.FileStream(configdir, FileMode.Open)
        Dim ser As New DataContractJsonSerializer(GetType(config))
        Dim cfg As config = Nothing
        Dim add_firewall As Boolean = False
        Try
            cfg = ser.ReadObject(fsTemp)
        Finally
            fsTemp.Close()
        End Try
        If cfg Is Nothing Then cfg = New config
        If cfg.Check_Validity() Then
            If System.IO.File.Exists(cfg.executable.Replace("/", "\")) Then
                Me.exec_path = cfg.executable.Replace("/", "\")
                Me.argv = " -v -c """ & configdir & """ " & cfg.argv
                ToolStripStatusLabel1.Text = "Using executable: " + Me.exec_path
                add_firewall = add_firewall Or firewall.Check_Exception_Exec(Me.exec_path)
                If cfg.obfs_level = 3 Then add_firewall = add_firewall Or firewall.Check_Exception_Exec(cfg.pt_exec)
                If add_firewall Then
                    If MsgBox("Required Windows Firewall Exceptions not found, adding now? Require Administrator privillege.", vbYesNo, "Adding Exceptions") = vbYes Then
                        firewall.Run_Add()
                    End If
                End If
            Else
                MsgBox("Executable not found, resetting config.", vbExclamation, "Executable Not Found")
                show_form2()
            End If

        Else
            Invalid()
        End If
    End Sub

    Private Sub Invalid()
        If MsgBox("Invalid config in " + configdir + " ." + vbNewLine + "Reset the config?",
                      vbYesNo, "Invalid Configuration File") = vbYes Then
            show_form2()
        End If
    End Sub

    Private Sub Process1_starting()
        Try
            Process1.CancelOutputRead()
        Catch

        End Try
        Process1.BeginOutputReadLine()
        Process1.WaitForExit()
    End Sub

    Private Sub Process2_starting()
        Try
            Process2.CancelOutputRead()
        Catch

        End Try
        Process2.BeginOutputReadLine()
        Process2.WaitForExit()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Stop_exec()
        Button1.Enabled = True
    End Sub

    Private Sub Stop_exec()
        If Process1 IsNot Nothing Then
            Try
                Process1.CancelOutputRead()
                If Not (Process1.HasExited) Then
                    killtree(Process1.Id)
                End If
            Catch
            End Try
            Process1.Dispose()
            Process1 = Nothing
        End If
    End Sub

    Private Sub Process1_OutputDataReceived_Process(sender As Object, e As DataReceivedEventArgs)
        Try
            RichTextBox1.AppendText(e.Data + vbCrLf)
            RichTextBox1.ScrollToCaret()
            If Not Me.proxy_set Then
                If e.Data.Contains("Listening to local services at ") Then
                    Dim temp As String()
                    Dim sub1, sub2 As String
                    temp = e.Data.Split(":")
                    sub1 = temp(3).Split(" ").Last()
                    sub2 = temp(4).Split(" ").First().Trim(".")
                    ProxyConfig.SetProxy(sub1, CUInt(sub2))
                    Me.proxy_set = True
                End If
            End If
        Catch

        End Try
        
    End Sub

    Private Sub Process1_Exited(sender As Object, e As EventArgs)
        RichTextBox1.AppendText(vbCrLf + "Execution Terminated" + vbCrLf)
        RichTextBox1.ScrollToCaret()
        ToolStripStatusLabel1.Text = "Using executable: " + Me.exec_path
        ToolStripStatusLabel2.Text = "Not running"
        ProxyConfig.DisableProxy()
        Me.proxy_set = False
    End Sub

    Private Sub killtree(myId As Integer)
        Dim selectQuery As SelectQuery = New SelectQuery("Win32_Process")
        Dim searcher As New ManagementObjectSearcher(selectQuery)
        Dim aProcess As Process
        Dim flag As Boolean = False
        For Each proc As ManagementObject In searcher.Get
            If proc("ParentProcessId") = myId Or proc("ProcessId") = myId Then
                aProcess = System.Diagnostics.Process.GetProcessById(Int(proc("ProcessId")))
                aProcess.Kill()
            End If
        Next
    End Sub


    'TODO: Capture end of execution

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        Key_Gen(Me.exec_path)
    End Sub

    Public Sub Key_Gen(exec_path As String)
        If Process2 IsNot Nothing Then Process2.Dispose()
        Process2 = New Process
        AddHandler Process2.OutputDataReceived, AddressOf Process1_OutputDataReceived_Process
        AddHandler Process2.Exited, AddressOf Process1_Exited
        With Process2
            .EnableRaisingEvents = True
            .StartInfo.UseShellExecute = False
            .StartInfo.RedirectStandardOutput = True
            .StartInfo.CreateNoWindow = True
            .StartInfo.FileName = exec_path
            .StartInfo.Arguments = " -kg"
            .Start()
            RichTextBox1.AppendText("Generating Key, please wait..." + vbCrLf)
            RichTextBox1.ScrollToCaret()
        End With
        Dim runThread = New Thread(AddressOf Process2_starting)
        runThread.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        show_form2()
    End Sub

    Private Sub show_form2()
        Dim show_form As Form2 = New Form2
        show_form.ShowDialog()
    End Sub

End Class
