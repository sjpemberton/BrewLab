namespace Models

open Units
open Calculations
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

[<AutoOpen>]
module Ingredients =

    type HopType =
    |Pellet
    |Leaf
    |Extract

    type hop<[<Measure>] 'w> = {Name:string; Alpha:float<percentage>;}
    type HopAddition<[<Measure>] 'w>  = { Hop:hop<'w>; Weight:float<'w>; Time:float<minute>; Type:HopType}
    type adjunct<[<Measure>] 'w> = {Name:string;  Potential:float<gp/'w>; Description:string }
    type AdjunctAddition<[<Measure>] 'w> = {Adjunct:adjunct<'w>; Name:string; Weight:float<'w>;}
    type grain<[<Measure>] 'w> = {Name:string; Potential:float<gp/'w>; Colour:float<EBC>;}
    type GrainAddition<[<Measure>] 'w> = {Grain: grain<'w>; Weight:float<'w>}
    type Addition<[<Measure>] 'w> = {Name:string; Weight:float<'w>;}
    type yeast<[<Measure>] 't> = {Name:string; Attenuation:float<percentage>; TempRange: float<'t>*float<'t> }
    type water = {Name:string;} //chemical profile

    type Fermentable<[<Measure>] 'w> =
    | Grain of GrainAddition<'w>
    | Adjunct of AdjunctAddition<'w>

    type Ingredient =
    | Hop of HopAddition<g>
    | Addition of adjunct<g>
    | Fermentable of Fermentable<kg>

    
