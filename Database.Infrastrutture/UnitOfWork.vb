Imports System.Data.OleDb

Namespace Database.Infrastrutture

    Public Class UnitOfWork : Implements IDisposable

        Public cn As IDbConnection

        Public Sub New(stringaConnessione As String)
            cn = New SqlServerCe.SqlCeConnection(stringaConnessione)
            If cn.State = ConnectionState.Closed Then cn.Open()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' Per rilevare chiamate ridondanti

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: eliminare lo stato gestito (oggetti gestiti).
                End If
                If cn IsNot Nothing AndAlso Not cn.State = ConnectionState.Closed Then cn.Close()
                cn = Nothing
                ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di Finalize().
                ' TODO: impostare campi di grandi dimensioni su Null.
            End If
            disposedValue = True
        End Sub

        ' TODO: eseguire l'override di Finalize() solo se Dispose(disposing As Boolean) include il codice per liberare risorse non gestite.
        'Protected Overrides Sub Finalize()
        '    ' Non modificare questo codice. Inserire sopra il codice di pulizia in Dispose(disposing As Boolean).
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Questo codice viene aggiunto da Visual Basic per implementare in modo corretto il criterio Disposable.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Non modificare questo codice. Inserire sopra il codice di pulizia in Dispose(disposing As Boolean).
            Dispose(True)
            ' TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override di Finalize().
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace