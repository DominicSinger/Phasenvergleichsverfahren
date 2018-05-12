Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Math
Imports System.IO


Class MainWindow

    Dim rangex As Double, rangey As Double
    Dim viscol As New VisualCanvasCollection
    Dim ursprung As Point
    Dim mauszoomvor, mauszoomnach As Point
    Dim hadwheelzoom As Boolean
    Dim lamda As Double
    Dim showsinus As Boolean = False

    Dim stepy As Double = 2.2, stepx As Double = 2.2
    Dim pens, pens2, pens3, pens4, pens5 As Pen
    Dim typpen1, typpen2, typpen3, typpen4, typpen5 As Pen

    Dim rest As Double
    Dim strecke As Double

    Dim WithEvents myvc As New VisualCanvas


    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

        Randomize(CDbl(Day(Today).ToString + Month(Today).ToString + Hour(Now).ToString + Minute(Now).ToString + Second(Now).ToString))
        initalize_pens()

        ursprung.X = 250
        ursprung.Y = 50


        viscol.Add(myvc)

        send_rec_zeichnen()
        entfernung_zeichnen()
        tabzeichnen.Content = myvc

    End Sub



    Private Sub maintabcontrol_SizeChanged(ByVal sender As Object, ByVal e As System.Windows.SizeChangedEventArgs) Handles maintabcontrol.SizeChanged

        rangey = CInt(maintabcontrol.ActualHeight)
        rangex = CInt(maintabcontrol.ActualWidth)

        viscol.Item(0).rangey = rangey
        viscol.Item(0).rangex = rangex

    End Sub

    Private Sub maintabcontrol_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Input.MouseWheelEventArgs) Handles maintabcontrol.MouseWheel

        If tabzeichnen.IsSelected = False Then Exit Sub

        Dim mp As Point
        mp = e.GetPosition(myvc)

        mauszoomvor.X = (mp.X - ursprung.X) / stepx     'in Lokale Koordinaten
        mauszoomvor.Y = (mp.Y - ursprung.Y) / stepy

        If CInt(e.Delta) > 0 Then
            If stepy + 0.1 < 4 And stepy + 0.1 < 4 Then   'hineinzoomen ---> Abstände größer machen
                stepx += 0.1
                stepy += 0.1

                hadwheelzoom = True
            Else
                Exit Sub
            End If

        Else
            If stepy - 0.1 > 1 And stepy - 0.1 > 1 Then     'herauszoomen ---> Abstände kleiner machen
                stepx -= 0.1
                stepy -= 0.1

                hadwheelzoom = True
            Else
                Exit Sub
            End If
        End If

        mauszoomnach.X = (mp.X - ursprung.X) / stepx            'Delta ausgleichen --> zoomen am Mauszeiger
        mauszoomnach.Y = (mp.Y - ursprung.Y) / stepy

        Dim deltax, deltay As Double
        deltax = (mauszoomnach.X - mauszoomvor.X) * stepx
        deltay = (mauszoomnach.Y - mauszoomvor.Y) * stepy

        'ursprung.X += deltax
        'ursprung.Y += deltay

        neu_zeichnen()


    End Sub



    Sub initalize_pens()

        'Initialisiert alle Pens

        pens = New Pen              'schwarz, durchgezogen, rund
        pens.Brush = Brushes.Black
        pens.Thickness = 2
        pens.StartLineCap = PenLineCap.Round
        pens.EndLineCap = PenLineCap.Round

        pens2 = New Pen              'rot, durchgezogen, rund
        pens2.Brush = Brushes.Red
        pens2.Thickness = 2
        pens2.StartLineCap = PenLineCap.Round
        pens2.EndLineCap = PenLineCap.Round

        pens3 = New Pen             'rot, gestrichelt, flach
        pens3.Brush = Brushes.Red
        pens3.Thickness = 3
        pens3.DashStyle = DashStyles.Dash
        pens3.StartLineCap = PenLineCap.Flat
        pens3.EndLineCap = PenLineCap.Flat

        pens4 = New Pen              'blau, durchgezogen, rund
        pens4.Brush = Brushes.Blue
        pens4.Thickness = 2
        pens4.StartLineCap = PenLineCap.Round
        pens4.EndLineCap = PenLineCap.Round

        pens5 = New Pen              'orange, durchgezogen, rund
        pens5.Brush = Brushes.Orange
        pens5.Thickness = 2
        pens5.StartLineCap = PenLineCap.Round
        pens5.EndLineCap = PenLineCap.Round

        typpen1 = New Pen           'grün, durchgezogen, rund
        typpen1.Brush = Brushes.Green
        typpen1.Thickness = 15
        typpen1.StartLineCap = PenLineCap.Round
        typpen1.EndLineCap = PenLineCap.Round

        typpen2 = New Pen           'blau, durchgezogen, rund
        typpen2.Brush = Brushes.CornflowerBlue
        typpen2.Thickness = 15
        typpen2.StartLineCap = PenLineCap.Round
        typpen2.EndLineCap = PenLineCap.Round

        typpen3 = New Pen           'gelb, durchgezogen, rund
        typpen3.Brush = Brushes.Yellow
        typpen3.Thickness = 15
        typpen3.StartLineCap = PenLineCap.Round
        typpen3.EndLineCap = PenLineCap.Round

        typpen4 = New Pen           'rot, durchgezogen, rund
        typpen4.Brush = Brushes.Red
        typpen4.Thickness = 15
        typpen4.StartLineCap = PenLineCap.Round
        typpen4.EndLineCap = PenLineCap.Round

        typpen5 = New Pen           'schwarz, durchgezogen, rund
        typpen5.Brush = Brushes.Black
        typpen5.Thickness = 15
        typpen5.StartLineCap = PenLineCap.Round
        typpen5.EndLineCap = PenLineCap.Round

    End Sub


    Sub neu_zeichnen()
        myvc.ClearVisual()              'Neu zeichnen
        send_rec_zeichnen()
        entfernung_zeichnen()
        reflektor_zeichnen()

        If showsinus = True Then sinus_zeichnen()

        tabzeichnen.Content = myvc
    End Sub

    Sub entfernung_zeichnen()

        Dim p3, p4 As Point
        Dim p As New Point(-5 * stepx + ursprung.X, 20)
        Dim p2 As New Point(305 * stepx + ursprung.X, 20)

        p3 = New Point(0 + ursprung.X, 10)
        p4 = New Point(0 + ursprung.X, 30)

        myvc.AddVisual(create_line_visual(pens2, p, p2))
        myvc.AddVisual(create_line_visual(pens, p3, p4))
        myvc.AddVisual(create_text_visual(pens, "0", p4))

        For i As Integer = 0 To 20




            If i Mod 5 = 0 Then
                p3 = New Point((100 + i * 10) * stepx + ursprung.X, 10)
                p4 = New Point((100 + i * 10) * stepx + ursprung.X, 30)

                myvc.AddVisual(create_text_visual(pens, (100 + i * 10).ToString, p4))
            Else
                p3 = New Point((100 + i * 10) * stepx + ursprung.X, 15)
                p4 = New Point((100 + i * 10) * stepx + ursprung.X, 25)
            End If

            myvc.AddVisual(create_line_visual(pens, p3, p4))

        Next

    End Sub

    Sub send_rec_zeichnen()
        Dim p3, p4 As Point

        'Rahmen
        p3 = New Point(0 * stepx + ursprung.X, 95)
        p4 = New Point(-105 * stepx + ursprung.X, 95)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-105 * stepx + ursprung.X, 95)
        p4 = New Point(-105 * stepx + ursprung.X, 405)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-105 * stepx + ursprung.X, 405)
        p4 = New Point(0 * stepx + ursprung.X, 405)

        myvc.AddVisual(create_line_visual(pens, p3, p4))



        'Sender
        p3 = New Point(0 * stepx + ursprung.X, 125)
        p4 = New Point(-90 * stepx + ursprung.X, 125)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-90 * stepx + ursprung.X, 125)
        p4 = New Point(-90 * stepx + ursprung.X, 225)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-90 * stepx + ursprung.X, 225)
        p4 = New Point(0 * stepx + ursprung.X, 225)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(0 * stepx + ursprung.X, 125)
        p4 = New Point(0 * stepx + ursprung.X, 225)
        myvc.AddVisual(create_bogen_visual(pens, p3, p4, 0.8, 5, SweepDirection.Clockwise))
        myvc.AddVisual(create_bogen_visual(pens, p3, p4, 0.8, 5, SweepDirection.Counterclockwise))

        p4 = New Point(-60 * stepx + ursprung.X, 150)
        myvc.AddVisual(create_text_visual(pens, "Sender", p4))




        'Empfänger
        p3 = New Point(0 * stepx + ursprung.X, 275)
        p4 = New Point(-90 * stepx + ursprung.X, 275)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-90 * stepx + ursprung.X, 275)
        p4 = New Point(-90 * stepx + ursprung.X, 375)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(-90 * stepx + ursprung.X, 375)
        p4 = New Point(0 * stepx + ursprung.X, 375)

        myvc.AddVisual(create_line_visual(pens, p3, p4))

        p3 = New Point(0 * stepx + ursprung.X, 275)
        p4 = New Point(0 * stepx + ursprung.X, 375)
        myvc.AddVisual(create_bogen_visual(pens, p3, p4, 0.8, 5, SweepDirection.Clockwise))
        myvc.AddVisual(create_bogen_visual(pens, p3, p4, 0.8, 5, SweepDirection.Counterclockwise))

        p4 = New Point(-60 * stepx + ursprung.X, 300)
        myvc.AddVisual(create_text_visual(pens, "Empfänger", p4))

    End Sub

    Sub reflektor_zeichnen()

        If TextBox7.Text = "" Then Exit Sub

        Dim p As New Point(CDbl(TextBox7.Text) * stepx + ursprung.X, 100)
        Dim p2 As New Point(CDbl(TextBox7.Text) * stepx + ursprung.X, 400)
        Dim p3 As New Point(CDbl(TextBox7.Text) * stepx + 150 + ursprung.X, 250)

        myvc.AddVisual(create_line_visual(pens, p, p2))
        myvc.AddVisual(create_line_visual(pens, p, p3))
        myvc.AddVisual(create_line_visual(pens, p3, p2))

        Dim p4 As New Point(CDbl(TextBox7.Text) * stepx + 30 + ursprung.X, 240)
        myvc.AddVisual(create_text_visual(pens, "Reflektor", p4))

    End Sub

    Sub sinus_zeichnen()

        Dim testpoint As New List(Of Point)

        Dim x, xz, yz, yz2 As Double


        Dim lastx, lasty, lasty2 As Double

        Dim ende, entfernung As Double
        ende = CDbl(TextBox7.Text)
        Dim test As New List(Of Double)

        x = 0
        lastx = x / (2 * PI) * lamda * stepx + ursprung.X
        lasty = Sin(x) * 50 + ursprung.Y + 125
        lasty2 = -Sin(x) * 50 + ursprung.Y + 125
        entfernung = 0

        Do While entfernung < ende

            x += 0.2

            entfernung = x / (2 * PI) * lamda

            If entfernung > ende Then
                x = ende * (2 * PI) / lamda
            End If

            xz = x / (2 * PI) * lamda * stepx + ursprung.X
            yz = -Sin(x) * 50 + ursprung.Y + 125
            yz2 = Sin(x) * 50 + ursprung.Y + 125

            If lamda >= 5 Then
                myvc.AddVisual(create_line_visual(pens4, New Point(lastx, lasty), New Point(xz, yz)))
                myvc.AddVisual(create_line_visual(pens4, New Point(lastx, lasty2), New Point(xz, yz2)))
            End If

            lastx = xz
            lasty = yz
            lasty2 = yz2

        Loop

        entfernung = x / (2 * PI) * lamda

        Dim xzende As Double
        xzende = lastx
        lasty = Sin(x) * 50 + ursprung.Y + 275
        lasty2 = -Sin(x) * 50 + ursprung.Y + 275


        Do While entfernung > 0

            x += 0.2

            entfernung = 2 * ende - x / (2 * PI) * lamda

            If entfernung < 0 Then
                x = 2 * ende * (2 * PI) / lamda
            End If

            xz = 2 * xzende - x / (2 * PI) * lamda * stepx - ursprung.X
            yz = Sin(x) * 50 + ursprung.Y + 275
            yz2 = -Sin(x) * 50 + ursprung.Y + 275

            If lamda >= 5 Then
                myvc.AddVisual(create_line_visual(pens4, New Point(lastx, lasty), New Point(xz, yz)))
                myvc.AddVisual(create_line_visual(pens4, New Point(lastx, lasty2), New Point(xz, yz2)))
            End If

            lastx = xz
            lasty = yz
            lasty2 = yz2

        Loop

        rest = x / (2 * PI) * lamda

        strecke = rest
        rest = Round(rest Mod lamda, 3)


        If lamda < 5 Then
            myvc.AddVisual(create_text_visual(pens, "Nicht mehr darstellbar!", New Point(20 * stepx + ursprung.X, 245)))
            Exit Sub
        End If

        If rest <= ende And rest <> 0 Then
            Dim rec As New Rect

            rec.X = ursprung.X
            rec.Y = 275

            rec.Width = rest * stepx
            rec.Height = 100

            Dim b As New SolidColorBrush With {.Color = Colors.Orange, .Opacity = 0.5}
            myvc.AddVisual(create_rectangle_visual(b, Nothing, rec))

            If rest > 25 Then
                myvc.AddVisual(create_line_visual(pens, New Point(ursprung.X, 260), New Point(0.5 * rest * stepx - 20 + ursprung.X, 260)))
                myvc.AddVisual(create_line_visual(pens, New Point(0.5 * rest * stepx + 20 + ursprung.X, 260), New Point(rest * stepx + ursprung.X, 260)))

                myvc.AddVisual(create_line_visual(pens, New Point(ursprung.X, 250), New Point(ursprung.X, 270)))
                myvc.AddVisual(create_line_visual(pens, New Point(rest * stepx + ursprung.X, 250), New Point(rest * stepx + ursprung.X, 270)))

                myvc.AddVisual(create_text_visual(pens, "Δ λ", New Point(0.5 * rest * stepx - 10 + ursprung.X, 250)))
            Else
                myvc.AddVisual(create_text_visual(pens, "Δ λ", New Point(0.5 * rest * stepx - 10 + ursprung.X, 250)))
            End If
        ElseIf rest <> 0 Then
            Dim rec As New Rect

            rec.X = ursprung.X
            rec.Y = 275

            rec.Width = ende * stepx
            rec.Height = 100

            Dim b As New SolidColorBrush With {.Color = Colors.Orange, .Opacity = 0.5}
            myvc.AddVisual(create_rectangle_visual(b, Nothing, rec))


            Dim rec2 As New Rect

            rec2.X = (2 * ende - rest) * stepx + ursprung.X
            rec2.Y = 125

            rec2.Width = (rest - ende) * stepx
            rec2.Height = 100

            myvc.AddVisual(create_rectangle_visual(b, Nothing, rec2))

            myvc.AddVisual(create_line_visual(pens, New Point(ursprung.X, 260), New Point(0.5 * ende * stepx - 20 + ursprung.X, 260)))
            myvc.AddVisual(create_line_visual(pens, New Point(0.5 * ende * stepx + 20 + ursprung.X, 260), New Point(ende * stepx + ursprung.X, 260)))

            myvc.AddVisual(create_line_visual(pens, New Point(ursprung.X, 250), New Point(ursprung.X, 270)))


            myvc.AddVisual(create_text_visual(pens, "Δ λ", New Point(0.5 * ende * stepx - 10 + ursprung.X, 250)))



            myvc.AddVisual(create_line_visual(pens, New Point((2 * ende - rest) * stepx + ursprung.X, 110), New Point(0.5 * (ende - (2 * ende - rest)) * stepx - 20 + ursprung.X, 110)))
            myvc.AddVisual(create_line_visual(pens, New Point(0.5 * (ende - (2 * ende - rest)) * stepx + 20 + ursprung.X, 110), New Point(ende * stepx + ursprung.X, 110)))

            myvc.AddVisual(create_line_visual(pens, New Point(ursprung.X, 100), New Point(ursprung.X, 120)))


            myvc.AddVisual(create_text_visual(pens, "Δ λ", New Point(0.5 * ende * stepx - 10 + ursprung.X, 100)))

        End If

    End Sub


    Private Function create_line_visual(ByVal pens As Pen, ByVal p1 As Point, ByVal p2 As Point) As Visual

        Dim visual As New DrawingVisual
        Dim dc As DrawingContext = visual.RenderOpen()
        dc.DrawLine(pens, p1, p2)
        dc.Close()

        Return visual
    End Function

    Private Function create_text_visual(ByVal pens As Pen, ByVal text As String, ByVal p1 As Point) As Visual

        Dim visual As New DrawingVisual
        Dim dc As DrawingContext = visual.RenderOpen()
        dc.DrawText(New FormattedText(text, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface("Arial"), 14, pens.Brush), New Point(p1.X + stepx / 10, p1.Y + stepy / 10))
        dc.Close()

        Return visual
    End Function

    Private Function create_bogen_visual(ByVal pens As Pen, ByVal startpunkt As Point, ByVal endpunkt As Point, ByVal radiusx As Double, ByVal radiusy As Double, ByVal sweepdirection As SweepDirection)
        Dim visual As New DrawingVisual
        Dim dc As DrawingContext = visual.RenderOpen()

        Dim pathgeometry As New PathGeometry
        Dim pathfigure As New Media.PathFigure
        Dim psc As New PathSegmentCollection

        Dim bogen As New ArcSegment With {.Point = endpunkt, .Size = New Size(radiusx, radiusy), .SweepDirection = sweepdirection}

        psc.Add(bogen)
        pathfigure.Segments = psc
        pathfigure.StartPoint = startpunkt

        pathgeometry.Figures.Add(pathfigure)

        dc.DrawGeometry(Nothing, pens, pathgeometry)
        dc.Close()

        Return visual
    End Function

    Private Function create_rectangle_visual(ByVal farbe As Brush, ByVal rahmen As Pen, ByVal rec As Rect) As Visual

        Dim visual As New DrawingVisual
        Dim dc As DrawingContext = visual.RenderOpen()

        dc.DrawRectangle(farbe, rahmen, rec)
        dc.Close()

        Return visual
    End Function

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        lamda = CDbl(TextBox1.Text)
        showsinus = True
        neu_zeichnen()

        TextBox4.Text = get_text()

        If TextBox4.Text <> "" And TextBox5.Text <> "" And TextBox6.Text <> "" Then
            TextBox8.Text = Round(strecke, find_max_dec)
            TextBox9.Text = CDbl(TextBox8.Text) / 2
        End If

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        lamda = CDbl(TextBox2.Text)
        showsinus = True
        neu_zeichnen()

        TextBox5.Text = get_text()

        If TextBox4.Text <> "" And TextBox5.Text <> "" And TextBox6.Text <> "" Then
            TextBox8.Text = Round(strecke, find_max_dec)
            TextBox9.Text = CDbl(TextBox8.Text) / 2
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        lamda = CDbl(TextBox3.Text)
        showsinus = True
        neu_zeichnen()

        TextBox6.Text = get_text()

        If TextBox4.Text <> "" And TextBox5.Text <> "" And TextBox6.Text <> "" Then
            TextBox8.Text = Round(strecke, find_max_dec)
            TextBox9.Text = CDbl(TextBox8.Text) / 2
        End If

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button4.Click

        TextBox7.Text = Round(100 + Rnd() * (300 - 100 + 1), 3)
        showsinus = False
        neu_zeichnen()

        TextBox4.Text = ""
        TextBox5.Text = ""
        TextBox6.Text = ""
        TextBox8.Text = ""
        TextBox9.Text = ""

    End Sub

    Private Function get_text() As String

        Dim lamdastr As String = (lamda / 1000).ToString
        Dim nkomma As Integer = InStr(lamdastr, ",")
        Dim lenstr As Integer = Len(lamdastr)

        Dim text As String

        If nkomma <> 0 Then
            Dim decstellen As Integer = lenstr - nkomma
            nkomma = InStr(rest, ",")
            text = Strings.Left(Round(rest, decstellen), nkomma + decstellen)
        Else
            nkomma = InStr(rest, ",")

            Dim stelle As Integer = Len(rest) - nkomma - lenstr
            text = Strings.Left(Round(rest, 0), stelle)
        End If

        Return text
    End Function

    Private Function find_max_dec() As Integer

        Dim nkomma4 As Integer = InStr(TextBox4.Text, ",")
        Dim decstellen4 As Integer

        If nkomma4 <> 0 Then
            Dim lenstr4 As Integer = Len(TextBox4.Text)
            decstellen4 = lenstr4 - nkomma4
        Else
            decstellen4 = 0
        End If

        Dim nkomma5 As Integer = InStr(TextBox5.Text, ",")
        Dim decstellen5 As Integer

        If nkomma5 <> 0 Then
            Dim lenstr5 As Integer = Len(TextBox5.Text)
            decstellen5 = lenstr5 - nkomma5
        Else
            decstellen5 = 0
        End If


        Dim nkomma6 As Integer = InStr(TextBox6.Text, ",")
        Dim decstellen6 As Integer

        If nkomma6 <> 0 Then
            Dim lenstr6 As Integer = Len(TextBox6.Text)
            decstellen6 = lenstr6 - nkomma6
        Else
            decstellen6 = 0
        End If


        Dim dec As Integer

        If decstellen4 >= decstellen5 Then
            dec = decstellen4
        Else
            dec = decstellen5
        End If

        If decstellen6 > dec Then
            dec = decstellen6
        End If

        Return dec
    End Function

    Private Sub BtnAbout_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles BtnAbout.Click
        Dim w As New About()
        w.Show()

    End Sub
End Class
