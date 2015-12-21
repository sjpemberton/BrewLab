namespace Models

open Units
open Calculations
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module Ingredients =

    type HopType = // These need a ratio to adjust IBUs accordingly
    |Pellet
    |Leaf
    |Extract

    type Alpha = 
        | Alpha of float<percentage>
          member x.Value = let (Alpha v) = x in v             

    type TempRange<[<Measure>] 't> = TempRange of float<'t> * float<'t>
    
    //type Water = {}

    type Yeast<[<Measure>] 't> = 
        { Name:string
          Attenuation:float<percentage>
          TempRange:TempRange<'t> }

    type Fermentable =
    | Grain of Potential:float<gp/kg> * Colour:float<EBC> * Name:string
    | Adjunct of Potential:float<gp/kg> * Name:string

    type Hop = Hop of Alpha:Alpha * Name:string 
    type Additive = Additive of Name:string
    
    type HopAddition = 
        { Hop:Hop
          Weight:float<g>
          Time:float<minute>
          Type:HopType }

    type FermentableAddition = 
        { Fermentable:Fermentable 
          Weight:float<g> }

    type AdditiveAddition = 
        { Additive:Additive 
          Weight:float<g> }

    type Ingredient =
    | HopAddition of HopAddition
    | FermentableAddition of FermentableAddition
    | AdditiveAddition of AdditiveAddition
