namespace Converters

open Microsoft.FSharp.Reflection
open System

type DiscriminatedUnionText() =

    let DuToString value uniontype =
        match FSharpValue.GetUnionFields(value, uniontype) with
        | case, _ -> case.Name

    interface System.Windows.Data.IValueConverter with
        override this.Convert(value, targetType, parameter, culture) =
            DuToString value (value.GetType())
            |> box
            
        override this.ConvertBack(value, targetType, parameter, culture) =
            raise(NotImplementedException())