' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
'Imports RmonTronrec_UniveralWindows.Settings
Imports Windows.UI.Popups
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.ViewManagement
Imports Windows.ApplicationModel.Core
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
Imports System.Drawing.Color
Imports System.String

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Public WithEvents ReconnectTimer As DispatcherTimer
    Public WithEvents KeepreadingLoopTimer As DispatcherTimer
    Public WithEvents GetLineBGW As New BackgroundWorker
    Public WithEvents BackgroundWorker1 As New BackgroundWorker
    Public client As TcpClient
    Public stream As NetworkStream
    Public reader As StreamReader
    Public r() As String
    Public bordercolor As Color
    Public lapcountString As String
    Public RaceColor As Color
    Public RaceBrush As SolidColorBrush
    Public line As String = Nothing ' this in the in data from the telnet connection string
    Public WithEvents MainWindow As Windows.UI.Core.CoreWindow
    Public Sub New()
        InitializeComponent()
        MainWindow = Windows.UI.Core.CoreWindow.GetForCurrentThread()
        ReconnectTimer = New DispatcherTimer
        ReconnectTimer.Interval = TimeSpan.FromMilliseconds(500)
        KeepreadingLoopTimer = New DispatcherTimer
        KeepreadingLoopTimer.Interval = TimeSpan.FromMilliseconds(1)
        AddHandler ReconnectTimer.Tick, AddressOf Connect1
        AddHandler KeepreadingLoopTimer.Tick, AddressOf Keepreading
        AddHandler BackgroundWorker1.DoWork, AddressOf BackgroundWorker1_DoWork
        AddHandler GetLineBGW.DoWork, AddressOf GetLineBGW_DoWork
        Debug.WriteLine("New Sub Created")
    End Sub
    Private Sub Button1_Click(sender As Object, e As RoutedEventArgs) Handles button1.Click
        Connect1()
        Debug.WriteLine("Button one Pressed")
    End Sub
    Private Sub GetLineBGW_DoWork(sender As Object, e As DoWorkEventArgs) Handles GetLineBGW.DoWork
        Debug.WriteLine("GetLine Triggered")
        If stream.CanRead Then
            Try
                While line.Length > 0
                    If BackgroundWorker1.CancellationPending Then
                        BackgroundWorker1.Dispose()
                        GetLineBGW.Dispose()
                        reader.Close()
                        stream.Close()
                    End If
                    Try
                        line = reader.ReadLine()
                        Debug.WriteLine(line)
                        Dim r() As String = line.Split(",")
                        If r(0) = "$G" Then
                            If r(1) = "1" Then
                                lapcountString = r(3).ToString
                                Debug.WriteLine(line)
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
                            If r(5) = """Green """ Then

                                If Not RaceColor = Color.Green Then
                                    RaceColor = Color.Green
                                End If
                            ElseIf r(5) = """Yellow""" Then
                                If Not RaceColor = Color.Yellow Then ' Windows.UI.Colors.Green Then
                                    RaceColor = Color.Yellow
                                End If
                            ElseIf r(5) = """Red   """ Then
                                If Not RaceColor = Color.Red Then
                                    RaceColor = Color.Red
                                End If
                            ElseIf r(5) = """      """ Then
                                RaceColor = Color.Black
                            Else
                                RaceColor = Color.Black
                            End If
                        End If
                        stream.Flush()
                    Catch al As Exception
                        GetLineBGW.CancelAsync()
                        Debug.Print(al.ToString)
                    End Try
                End While
            Catch Whilee As Exception
                BackgroundWorker1.Dispose()
                GetLineBGW.Dispose()
                GetLineBGW.CancelAsync()
                reader.Close()
                stream.Close()
                KeepreadingLoopTimer.Stop()
                ReconnectTimer.Start()
            End Try
        End If
    End Sub

    Private Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        BackgroundWorker1.WorkerSupportsCancellation = True
        BackgroundWorker1.WorkerReportsProgress = True
        GetLineBGW.WorkerSupportsCancellation = True
        GetLineBGW.WorkerReportsProgress = True
    End Sub

    Private Sub Page_KeyDown(sender As Windows.UI.Core.CoreWindow, e As Windows.UI.Core.KeyEventArgs) Handles MainWindow.KeyDown
        If e.VirtualKey = Windows.System.VirtualKey.S Then
            SettingsPanel.Visibility = Visibility.Visible
        End If
        If e.VirtualKey = Windows.System.VirtualKey.Escape Then
            SettingsPanel.Visibility = Visibility.Collapsed
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode()
            CoreApplication.GetCurrentView.TitleBar.ExtendViewIntoTitleBar = True

        End If
    End Sub
    Private Sub MainPage_KeyUp(sender As Object, e As KeyRoutedEventArgs) Handles Me.KeyUp
        If e.Key = Windows.System.VirtualKey.S Then
            SettingsPanel.Visibility = Visibility.Visible
        End If
        If e.Key = Windows.System.VirtualKey.Escape Then
            SettingsPanel.Visibility = Visibility.Collapsed
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode()
            CoreApplication.GetCurrentView.TitleBar.ExtendViewIntoTitleBar = True
        End If
    End Sub
    Private Sub Exitbut_Click(sender As Object, e As RoutedEventArgs) Handles exitbut.Click
        End
    End Sub
    Public Sub Connect1()
        Debug.WriteLine("Connect1 Triggered")
        Try
            ConnectionStatus.Text = "Connecting..."
            Dim port As Int32 = 50000
            Dim client As New TcpClient(textBox1.Text, port) 'textBox1.Text
            stream = client.GetStream()
            reader = New StreamReader(stream)
            client.ReceiveTimeout = False
            Dim Data = New [Byte](256) {}
            Dim bytes As Int32 = 0
            Dim responseData As [String] = [String].Empty
            ConnectionStatus.Text = "Reachable \ Please Wait... : " + textBox1.Text
            If stream Is Nothing Then
                Throw New Exception("Stream is nothing!?!")
            End If
            stream.Flush()
            Try
                ConnectionStatus.Text = "Trying to read data..."
                line = reader.ReadLine()
            Catch
                ConnectionStatus.Text = "Something Went Wrong"
            Finally
            End Try
            KeepreadingLoopTimer.Start()
            If Not BackgroundWorker1.IsBusy = True Then
                BackgroundWorker1.RunWorkerAsync()

            End If
        Catch ex As SocketException
            ConnectionStatus.Text = "Conncection Failed: " & ex.ToString
        Finally
        End Try
    End Sub
    Public Async Sub StartKeepReading()
        If Not Me.Dispatcher.HasThreadAccess Then
            Debug.Print("StartKeepReading Doesn't have Thread access" + vbCrLf)
            Try
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf Keepreading)
            Catch
                Debug.Print("keepreading cast caught")
            End Try
        Else
        End If
    End Sub
    'Public Sub OldWhileLoop()
    '    Debug.WriteLine("OldWhileLoop triggered")

    '    If line.Length > 0 And stream.CanRead Then
    '        Debug.WriteLine("Line is > 0 AND Stream Can Read")
    '        If Not lapcountString Is Nothing Then
    '            LapCount.Text = lapcountString
    '        End If

    '        Bindings.Update()

    '        'Threading.Thread.Sleep(10)
    '        'UpdateLayout()
    '        'Task.WaitAll()
    '        'Await Task.Yield
    '        'Application.DoEvents()
    '        ConnectionStatus.Text = "Connected"
    '        'Await Task.Yield
    '        'Dim newline As String = Nothing 'Creates nothing for 'newline'
    '        'If PosNTop.Checked Then '        'Checks if the checkbox is checked
    '        'newline = vbCrLf '           If checked, move the POS number above the Car number
    '        'Else '                             
    '        'newline = Nothing '
    '        'End If

    '        If BackgroundWorker1.CancellationPending Then
    '            BackgroundWorker1.Dispose()
    '            GetLineBGW.Dispose()
    '            reader.Close()
    '            stream.Close()

    '            ConnectionStatus.Text = "Disconnected"
    '            ''!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '            ''timer needs stopped that started OldWHileLoop

    '            'If dbgen.Checked Then
    '            'TextBox2.Clear()
    '            'End If

    '        End If

    '        'If CLng((DateTime.UtcNow - epoch).TotalMilliseconds) - millis >= 20 Then
    '        'millis = CLng((DateTime.UtcNow - epoch).TotalMilliseconds)
    '        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''line = reader.ReadLine()

    '        'If stream.CanRead Then
    '        'Debug.WriteLine("Stream Can Read")
    '        'Try
    '        '    line = reader.ReadLine()
    '        'Catch al As Exception
    '        '    'GetLineBGW.CancelAsync()
    '        '    Debug.Print(al.ToString)
    '        'End Try
    '        'End If


    '        'Copying getling below to above line at "Trying to read data"
    '        If Not GetLineBGW.IsBusy = True Then
    '            ' Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Function()
    '            GetLineBGW.RunWorkerAsync()
    '            '  Return True
    '            'End Function)
    '            ' Await Task.Run(Function()
    '            ''If Not GetLineBGW.IsBusy = True Then
    '            'GetLineBGW.RunWorkerAsync()
    '            'Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Function()
    '            ''GetLineBGW.RunWorkerAsync()
    '            '                                                                            Return GetLineBGW
    '            'End Function)

    '        End If
    '        '                  Return GetLineBGW

    '        'End Function)
    '        'End If


    '        'UpdateLayout()
    '        'Await Task.Yield
    '        'Task.
    '        'Application.DoEvents()
    '        'TextBox2.Text = line
    '        ' DIM r STARTS HERE
    '        Dim r() As String = line.Split(",")
    '        If r(0) = "$G" Then
    '            If r(1) = "1" Then
    '                'Dim settingspage As MainPage = New MainPage()

    '                'Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, Function()
    '                'LapCount.Text = r(3).ToString '' changing to string
    '                '               
    '                lapcountString = r(3).ToString
    '                LapCount.Text = r(3).ToString
    '                Bindings.Update()
    '                'Try
    '                'Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, AddressOf UpdateUI)

    '                'Catch
    '                'Debug.Print("UpdateUI cast caught")
    '                'End Try


    '                '''''''''''BackgroundWorker1.ReportProgress(0, LapCount.Text)
    '                ' BackgroundWorker1.ReportProgress(0)
    '                '''''''''''Await Task.Yield()
    '                '                                                                           ' Await Task.Run(Function()
    '                '                                                                           'UpdateUI()
    '                '                                                                           Return Nothing

    '                '                                                                       End Function)
    '                'Dispatcher.ProcessEvents(Windows.UI.Core.CoreProcessEventsOption.ProcessOneAndAllPending)
    '                'Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, Function()
    '                'LapCount.Text = lapcountString
    '                '
    '                'UpdateUI()
    '                'Task.Yield()
    '                'Return Task.Delay(5)
    '                'End Function)

    '                'Return True
    '                'End Function)
    '                'LapCount.Text = r(3)
    '                'LapCount.Text = r(3)
    '                'PlayerForm.Pos1.Text = "1  " + newline + r(2).Substring(1, r(2).Length - 2)
    '                ' Await Task.Yield
    '            End If
    '            If r(1) = "2" Then
    '                'PlayerForm.Pos2.Text = "2  " + newline + r(2).Substring(1, r(2).Length - 2) 'IndexOf("\""")
    '            End If
    '            If r(1) = "3" Then
    '                'PlayerForm.Pos3.Text = "3  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "4" Then
    '                'PlayerForm.Pos4.Text = "4  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "5" Then
    '                ' PlayerForm.Pos5.Text = "5  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "6" Then
    '                'PlayerForm.Pos6.Text = "6  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "7" Then
    '                'PlayerForm.Pos7.Text = "7  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "8" Then
    '                'PlayerForm.Pos8.Text = "8  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If
    '            If r(1) = "9" Then
    '                'PlayerForm.Pos9.Text = "9  " + newline + r(2).Substring(1, r(2).Length - 2)
    '            End If

    '        ElseIf r(0) = "$F" Then
    '            'Dim settingspage As MainPage = New MainPage()
    '            If r(5) = """Green """ Then

    '                If Not PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Green) Then ' Windows.UI.Colors.Green Then
    '                    'PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Green)
    '                    'RaceColor = New SolidColorBrush(Windows.UI.Colors.Green)
    '                    'PlayerFormGrid.InvalidateMeasure()
    '                    'AddHandler PlayerForm.Paint, AddressOf Playerorm.PlayerForm_Paint
    '                    'Me.Controls.Add(PlayerForm)
    '                    'ControlPaint.DrawBorder(e.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
    '                    'PlayerForm.Paint()
    '                    'MainPage.PlayerFormGrid.Invalidate()
    '                End If

    '            ElseIf r(5) = """Yellow""" Then
    '                If Not PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Yellow) Then ' Windows.UI.Colors.Green Then
    '                    'PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Yellow)
    '                    'RaceColor = New SolidColorBrush(Windows.UI.Colors.Yellow)
    '                    ' PlayerFormGrid.InvalidateMeasure()
    '                    'ControlPaint.DrawBorder(e.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
    '                    'MainPage.PlayerFormGrid.Invalidate()
    '                End If
    '            ElseIf r(5) = """Red   """ Then
    '                If Not PlayerFormGrid.Background Is New SolidColorBrush(Windows.UI.Colors.Red) Then ' Windows.UI.Colors.Green Then
    '                    'PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Red)
    '                    ' RaceColor = New SolidColorBrush(Windows.UI.Colors.Red)
    '                    'PlayerFormGrid.InvalidateMeasure()
    '                    'ControlPaint.DrawBorder(.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
    '                    'MainPage.PlayerFormGrid.Invalidate()
    '                End If
    '            ElseIf r(5) = """      """ Then
    '                'PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Black)
    '                'RaceColor = New SolidColorBrush(Windows.UI.Colors.Black)
    '                'PlayerFormGrid.InvalidateMeasure()
    '                'ControlPaint.DrawBorder(.Graphics, PlayerForm.DisplayRectangle, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid, bordercolor, 5, ButtonBorderStyle.Solid)
    '                'MainPage.PlayerFormGrid.Invalidate()
    '            Else
    '                ' PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Colors.Black)
    '                'RaceColor = New SolidColorBrush(Windows.UI.Colors.Black)
    '            End If

    '            'Else : End If 'end if for millis


    '        End If
    '        'If dbgen.Checked Then
    '        'TextBox2.Text = line
    '        'End If
    '        'Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdateUI)
    '        stream.Flush()
    '        'MessageBox.Show(reader.EndOfStream)
    '        'TextBox2.Update()

    '        'System.Threading.Thread.Sleep(100)

    '    End If

    'End Sub
    Public Async Sub Keepreading()
        Debug.WriteLine("Keepreading has been called")
        Dim epoch = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        Dim millis = CLng((DateTime.UtcNow - epoch).TotalMilliseconds)
        If Not Me.Dispatcher.HasThreadAccess Then
            Debug.Print("Doesn't have Thread access" + vbCrLf)
            Try
                Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf Keepreading)
            Catch
                Debug.Print("keepreading cast caught")
            End Try
        Else
            If Not line Is Nothing Then
                Try
                    ReconnectTimer.Stop()
                    Dim timed As Long = 0
                    ConnectionStatus.Text = "Connected"
                    If Not GetLineBGW.IsBusy = True Then
                        GetLineBGW.RunWorkerAsync()
                    End If
                    If Not lapcountString Is Nothing Then
                        LapCount.Text = lapcountString
                    End If
                    If Not RaceColor = Nothing Then
                        PlayerFormGrid.Background = New SolidColorBrush(Windows.UI.Color.FromArgb(RaceColor.A, RaceColor.R, RaceColor.G, RaceColor.B))
                    End If
                    Await Task.Yield
                Catch e As Exception
                    ConnectionStatus.Text = "error"
                    BackgroundWorker1.Dispose()
                    GetLineBGW.Dispose()
                    GetLineBGW.CancelAsync()
                    reader.Close()
                    stream.Close()
                    ConnectionStatus.Text = "Disconnected with Error"
                    TextBox2.Text = e.ToString
                    KeepreadingLoopTimer.Stop()
                    ReconnectTimer.Start()
                End Try
            End If
        End If
    End Sub
    Public Sub UpdateUI()
        If Not lapcountString = Nothing Then
            LapCount.Text = lapcountString
            Bindings.Update()
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As RoutedEventArgs) Handles button2.Click
        BackgroundWorker1.CancelAsync()
        BackgroundWorker1.Dispose()
        GetLineBGW.Dispose()
        GetLineBGW.CancelAsync()
        KeepreadingLoopTimer.Stop()
        ReconnectTimer.Stop()
        ConnectionStatus.Text = "Disconnected"
        lapcountString = "..."
        LapCount.Text = "..."
        Bindings.Update()
    End Sub
    Public Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Keepreading()
    End Sub
End Class
