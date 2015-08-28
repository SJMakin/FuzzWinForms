Public Class LevenshteinDistance

    Public Shared Function Compute(s As String, t As String) As Integer
        If String.IsNullOrEmpty(s) Then
            If String.IsNullOrEmpty(t) Then
                Return 0
            End If
            Return t.Length
        End If

        If String.IsNullOrEmpty(t) Then
            Return s.Length
        End If

        Dim n As Integer = s.Length
        Dim m As Integer = t.Length
        Dim d As Integer(,) = New Integer(n, m) {}

        ' initialize the top and right of the table to 0, 1, 2, ...
        Dim i As Integer = 0
        While i <= n


            d(i, 0) = System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While
        Dim j As Integer = 1
        While j <= m


            d(0, j) = System.Math.Max(System.Threading.Interlocked.Increment(j), j - 1)
        End While

        For i As Integer = 1 To n
            For j As Integer = 1 To m
                Dim cost As Integer = If((t(j - 1) = s(i - 1)), 0, 1)
                Dim min1 As Integer = d(i - 1, j) + 1
                Dim min2 As Integer = d(i, j - 1) + 1
                Dim min3 As Integer = d(i - 1, j - 1) + cost
                d(i, j) = Math.Min(Math.Min(min1, min2), min3)
            Next
        Next
        Return d(n, m)
    End Function


End Class
