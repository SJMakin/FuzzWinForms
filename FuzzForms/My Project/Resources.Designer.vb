﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.34014
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("FuzzForms.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to # Taken from https://github.com/minimaxir/big-list-of-naughty-strings
        '''#	Reserved Strings
        '''#
        '''#	Strings which may be used elsewhere in code
        '''
        '''undefined
        '''undef
        '''null
        '''NULL
        '''nil
        '''NIL
        '''true
        '''false
        '''True
        '''False
        '''None
        '''
        '''#	Numeric Strings
        '''#
        '''#	Strings which can be interpreted as numeric
        '''
        '''0
        '''1
        '''1.00
        '''$1.00
        '''1/2
        '''1E2
        '''1E02
        '''1E+02
        '''-1
        '''-1.00
        '''-$1.00
        '''-1/2
        '''-1E2
        '''-1E02
        '''-1E+02
        '''1/0
        '''0/0
        '''0.00
        '''0..0
        '''.
        '''0.0.0
        '''0,00
        '''0,,0
        ''',
        '''0,0,0
        '''0.0/0
        '''1.0/0.0
        '''0.0/0.0
        '''1,0/0,0
        '''0,0/0,0
        '''--1
        '''-
        '''-.
        '''-,
        '''99999999999999999999999 [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property NaughtyStrings() As String
            Get
                Return ResourceManager.GetString("NaughtyStrings", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
