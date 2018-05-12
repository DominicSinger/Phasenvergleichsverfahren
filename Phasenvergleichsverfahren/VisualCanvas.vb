Imports System.Windows
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.ComponentModel
Imports System.Windows.Media.Imaging
Imports System.Collections.ObjectModel


Public Class VisualCanvas

    Inherits Canvas

    Private _visuals As New List(Of Visual)
    Private _rangex, _rangey As Integer
    Private _filename As String

    Protected Overrides ReadOnly Property VisualChildrenCount() As Integer

        Get
            Return _visuals.Count
        End Get

    End Property

    Protected Overrides Function GetVisualChild(ByVal index As Integer) As System.Windows.Media.Visual
        Return _visuals(index)
    End Function

    ''' <summary>
    ''' Fügt ein visual hinzu.
    ''' </summary>
    ''' <param name="visual"></param>
    ''' <remarks></remarks>
    Sub AddVisual(ByVal visual As Visual)

        _visuals.Add(visual)
        MyBase.AddVisualChild(visual)
        MyBase.AddLogicalChild(visual)

    End Sub

    ''' <summary>
    ''' Löscht visual.
    ''' </summary>
    ''' <param name="visual"></param>
    ''' <remarks></remarks>
    Sub DeleteVisual(ByVal visual As Visual)

        _visuals.Remove(visual)
        MyBase.RemoveVisualChild(visual)
        MyBase.RemoveLogicalChild(visual)

    End Sub

    ''' <summary>
    ''' Löscht alle Visuals.
    ''' </summary>
    ''' <remarks></remarks>
    Sub ClearVisual()

        For i As Integer = _visuals.Count - 1 To 0 Step -1

            MyBase.RemoveVisualChild(_visuals(i))
            MyBase.RemoveLogicalChild(_visuals(i))
            _visuals.Remove(_visuals(i))

        Next

    End Sub

    ''' <summary>
    ''' Ersetzt visual(index) durch visual
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <param name="visual"></param>
    ''' <remarks></remarks>
    Sub ReplaceIndexVisual(ByVal Index As Integer, ByVal visual As Visual)

        DeleteVisual(_visuals(Index))
        AddVisual(visual)

    End Sub

    ''' <summary>
    ''' Ersetzt visual1 durch visual2
    ''' </summary>
    ''' <param name="visual1"></param>
    ''' <param name="visual2"></param>
    ''' <remarks></remarks>
    Sub ReplaceVisualVisual(ByVal visual1 As Visual, ByVal visual2 As Visual)

        DeleteVisual(visual1)
        AddVisual(visual2)

    End Sub

    ''' <summary>
    ''' Zähler alle Visuals
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function CountVisual() As Integer
        Return _visuals.Count
    End Function

    ''' <summary>
    ''' Gibt den Index von visual zurück
    ''' </summary>
    ''' <param name="visual"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function IndexOfVisual(ByVal visual As Visual) As Integer
        Return _visuals.IndexOf(visual)
    End Function

    ''' <summary>
    ''' Gibt visual mit angegeben index zurück.
    ''' </summary>
    ''' <param name="index"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetVisual(ByVal index As Integer) As Visual
        Return (_visuals(index))
    End Function

    ''' <summary>
    ''' Liefert das letzte Visual.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function LastVisual() As Visual
        Return _visuals.Last
    End Function

    ''' <summary>
    ''' Liefer das erste Visual.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function FirstVisual() As Visual
        Return _visuals.First
    End Function

    ''' <summary>
    ''' Speichert Breite der Zeichnungsfläche.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property rangex As Integer

        Get
            Return _rangex
        End Get

        Set(ByVal value As Integer)
            _rangex = value
        End Set

    End Property

    ''' <summary>
    ''' Speichert Höhe der Zeichnungsfläche
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property rangey As Integer

        Get
            Return _rangey
        End Get

        Set(ByVal value As Integer)
            _rangey = value
        End Set

    End Property

    Public Property filename As String

        Get
            Return _filename
        End Get

        Set(ByVal value As String)
            _filename = value
        End Set

    End Property

    Public Shared Operator +(ByVal lh As VisualCanvas, ByVal rh As VisualCanvas) As VisualCanvas

        Dim sumvis As New VisualCanvas

        For i As Integer = 0 To lh.VisualChildrenCount - 1
            sumvis.AddVisual(lh.GetVisual(i))
        Next

        For i As Integer = 0 To rh.VisualChildrenCount - 1
            sumvis.AddVisual(rh.GetVisual(i))
        Next

        If sumvis.VisualChildrenCount = lh.VisualChildrenCount + rh.VisualChildrenCount Then
            Return sumvis
        Else
            Return Nothing
        End If

    End Operator

End Class

Public Class VisualCanvasCollection
    Inherits ObservableCollection(Of VisualCanvas)
End Class
