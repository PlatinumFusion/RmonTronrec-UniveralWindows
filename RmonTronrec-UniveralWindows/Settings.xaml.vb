Imports System.ComponentModel
Imports System.Text
Imports System.Net
Imports System.Net.Sockets
Imports System
Imports System.IO
Imports System.Linq
Imports System.Threading
Imports Microsoft.VisualBasic
Imports System.Drawing
Imports System.String
Imports RmonTronrec_UniveralWindows.MainPage
' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Settings
    Inherits Page
    Public WithEvents ReconnectTimer As DispatcherTimer
    Public WithEvents GetLineBGW As New BackgroundWorker
    Public WithEvents BackgroundWorker1 As New BackgroundWorker
    Public stream As NetworkStream
    Public reader As StreamReader
    Public r() As String
    Public bordercolor As Color
    Public line As String = Nothing ' this in the in data from the telnet connection string

    'Public WithEvents MainWindow As Windows.UI.Core.CoreWindow
    'Sub New()
    '    MainWindow = Windows.UI.Core.CoreWindow.GetForCurrentThread()
    'End Sub
    Private Sub Exitbut_Click(sender As Object, e As RoutedEventArgs) Handles Exitbut.Click
        End
    End Sub

    Private Sub Settings_KeyDown(sender As Object, e As KeyRoutedEventArgs) Handles Me.KeyDown
        If e.Key = Windows.System.VirtualKey.Escape Then
            '' Open Settings
            'RmonTronrec_UniveralWindows.Settings
            Frame.Navigate(GetType(MainPage))
        End If
    End Sub
    Public Sub New()
        ReconnectTimer = New DispatcherTimer
        ReconnectTimer.Interval = TimeSpan.FromMilliseconds(50)
        AddHandler ReconnectTimer.Tick, AddressOf Connect1
        'ReconnectTimer.Start() '' This is where I left off 5/8/19
        AddHandler BackgroundWorker1.DoWork, AddressOf BackgroundWorker1_DoWork
        AddHandler GetLineBGW.DoWork, AddressOf GetLineBGW_DoWork

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub Button1_Click(sender As Object, e As RoutedEventArgs) Handles button1.Click
        Connect1()
        If Not BackgroundWorker1.IsBusy = True Then
            BackgroundWorker1.RunWorkerAsync()

        End If
    End Sub
    Private Sub GetLineBGW_DoWork(sender As Object, e As DoWorkEventArgs) Handles GetLineBGW.DoWork
        If stream.CanRead Then
            Try
                line = reader.ReadLine()
            Catch
                GetLineBGW.CancelAsync()
            End Try
        End If
    End Sub

    'Private Sub Settings_KeyDown(sender As Windows.UI.Core.CoreWindow, e As Windows.UI.Core.KeyEventArgs) Handles MainWindow.KeyDown

    'If e.Key = Windows.System.VirtualKey.Escape Then
    'If e.VirtualKey = Windows.System.VirtualKey.Escape Then
    '' Open Settings
    'RmonTronrec_UniveralWindows.Settings
    ' Frame.Navigate(GetType(MainPage))
    'Frame.Navigate = RmonTronrec_UniveralWindows.Settings
    'End If
    'End Sub
    Public Sub Connect1()
        ' Dim server As TcpClient
        'server = Nothing
        'ConnectionStatus.Visible = True

        'If button1.Content = "Connect" Then
        Try

                ConnectionStatus.Text = "Connecting..."
                Dim port As Int32 = 50000
                Dim client As New TcpClient(textBox1.Text, port)
                stream = client.GetStream()
                reader = New StreamReader(stream)
                client.ReceiveTimeout = False
                Dim Data = New [Byte](256) {}
                Dim bytes As Int32 = 0
                Dim responseData As [String] = [String].Empty
                ConnectionStatus.Text = "Reachable \ Please Wait..."
                'If dbgen.Checked Then
                'TextBox2.Text = reader.ReadLine() + vbCrLf 'No DEBUG set on this program
                'End If
                stream.Flush()
            Catch ex As SocketException
                ConnectionStatus.Text = "Conncection Failed"
            Finally
            End Try
        'Else
        'End If
    End Sub

    Public Async Function Keepreading() As Task

        Dim epoch = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        Dim millis = CLng((DateTime.UtcNow - epoch).TotalMilliseconds)
        'If Me.InvokeRequired Then
        If Not Me.Dispatcher.HasThreadAccess Then
            ''Can't invoke here?????
            'Await New Keepreading()
            'Me.Invoke(New MethodInvoker(AddressOf Keepreading))
            'Me.Dispatcher.ProcessEvents(AddressOf Keepreading)
            'Me.Dispatcher.
            'Me.Dispatcher.invoke(AddressOf Keepreading)
            Await Me.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Keepreading)
        Else
            'TextBox2.Text = TextBox2.Text + reader.ReadLine() + vbCrLf
            'While True
            'Dim p As Integer = reader.Peek
            'MessageBox.Show(reader.Peek)
            'reader.ReadLine
            'reader.ReadLine()

            Try
                line = reader.ReadLine()
            Catch
            Finally

            End Try

            ''If NOT LINE IS NOTHING STARTS GHE
            If Not line Is Nothing Then
                Try

                    ReconnectTimer.Stop()
                    'ReconnectTimer.IsEnabled = False
                    Dim timed As Long = 0

                    While line.Length > 0

                        'Application.DoEvents()
                        ConnectionStatus.Text = "Connected"
                        Dim newline As String = Nothing 'Creates nothing for 'newline'
                        'If PosNTop.Checked Then '        'Checks if the checkbox is checked
                        'newline = vbCrLf '           If checked, move the POS number above the Car number
                        'Else '                             
                        'newline = Nothing '
                        'End If

                        If BackgroundWorker1.CancellationPending Then
                            BackgroundWorker1.Dispose()
                            GetLineBGW.Dispose()
                            reader.Close()
                            stream.Close()
                            ConnectionStatus.Text = "Disconnected"
                            'If dbgen.Checked Then
                            'TextBox2.Clear()
                            'End If
                            Exit While
                        End If

                        'If CLng((DateTime.UtcNow - epoch).TotalMilliseconds) - millis >= 20 Then
                        'millis = CLng((DateTime.UtcNow - epoch).TotalMilliseconds)
                        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''line = reader.ReadLine()
                        If Not GetLineBGW.IsBusy = True Then
                            GetLineBGW.RunWorkerAsync()

                        End If

                        'Application.DoEvents()
                        'TextBox2.Text = line
                        ' DIM r STARTS HERE
                        Dim r() As String = line.Split(",")
                        If r(0) = "$G" Then
                            If r(1) = "1" Then
                                Dim settingspage As MainPage = New MainPage()
                                settingspage.LapCount.Text = r(3)
                                'LapCount.Text = r(3)
                                'PlayerForm.Pos1.Text = "1  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "2" Then
                                'PlayerForm.Pos2.Text = "2  " + newline + r(2).Substring(1, r(2).Length - 2) 'IndexOf("\""")
                            End If
                            If r(1) = "3" Then
                                'PlayerForm.Pos3.Text = "3  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "4" Then
                                'PlayerForm.Pos4.Text = "4  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "5" Then
                                ' PlayerForm.Pos5.Text = "5  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "6" Then
                                'PlayerForm.Pos6.Text = "6  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "7" Then
                                'PlayerForm.Pos7.Text = "7  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "8" Then
                                'PlayerForm.Pos8.Text = "8  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                            If r(1) = "9" Then
                                'PlayerForm.Pos9.Text = "9  " + newline + r(2).Substring(1, r(2).Length - 2)
                            End If
                        ElseIf r(0) = "$F" Then
                            Dim settingspage As MainPage = New MainPage()
                            If r(5) = """Green """ Then

                                If Not settingspage.PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Green) Then ' Windows.UI.Colors.Green Then
                                    settingspage.PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Green)
                                    'AddHandler PlayerForm.Paint, AddressOf Playerorm.PlayerForm_Paint
                                    'Me.Controls.Add(PlayerForm)
                                    'ControlPaint.DrawBorder(e.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
                                    'PlayerForm.Paint()
                                    'MainPage.PlayerFormGrid.Invalidate()
                                End If

                            ElseIf r(5) = """Yellow""" Then
                                If Not settingspage.PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Yellow) Then ' Windows.UI.Colors.Green Then
                                    settingspage.PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Yellow)
                                    'ControlPaint.DrawBorder(e.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
                                    'MainPage.PlayerFormGrid.Invalidate()
                                End If
                            ElseIf r(5) = """Red   """ Then
                                If Not settingspage.PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Red) Then ' Windows.UI.Colors.Green Then
                                    settingspage.PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Red)
                                    'ControlPaint.DrawBorder(.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
                                    'MainPage.PlayerFormGrid.Invalidate()
                                End If
                            ElseIf r(5) = """      """ Then
                                settingspage.PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Black)
                                'ControlPaint.DrawBorder(.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
                                'MainPage.PlayerFormGrid.Invalidate()
                            Else
                                settingspage.PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Black)
                            End If
                            'Else : End If 'end if for millis


                        End If
                        'If dbgen.Checked Then
                        'TextBox2.Text = line
                        'End If

                        stream.Flush()
                        'MessageBox.Show(reader.EndOfStream)
                        'TextBox2.Update()


                        'System.Threading.Thread.Sleep(100)


                    End While
                Catch e As Exception
                    ConnectionStatus.Text = "error"
                    BackgroundWorker1.Dispose()
                    GetLineBGW.Dispose()
                    GetLineBGW.CancelAsync()
                    reader.Close()
                    stream.Close()
                    ConnectionStatus.Text = "Disconnected with Error"
                    'If dbgen.Checked Then
                    '    TextBox2.Clear()
                    'End If
                    'MessageBox.Show(e.ToString)
                    'ReconnectTimer.Enabled = True
                    ReconnectTimer.Start()

                End Try
            End If

        End If
    End Function

    Private Sub Button2_Click(sender As Object, e As RoutedEventArgs) Handles button2.Click
        BackgroundWorker1.CancelAsync()
    End Sub
    Public Async Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Await Keepreading()
    End Sub

    Private Sub Settings_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        BackgroundWorker1.WorkerSupportsCancellation = True
        BackgroundWorker1.WorkerReportsProgress = True
    End Sub
End Class
