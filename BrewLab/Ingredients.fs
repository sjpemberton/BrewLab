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

//    type adjunct<[<Measure>] 'w> = {Name:string;  Potential:float<gp/'w>; Description:string }
//    type AdjunctAddition<[<Measure>] 'w> = {Adjunct:adjunct<'w>; Name:string; Weight:float<'w>;}
//    type grain<[<Measure>] 'w> = {Name:string; Potential:float<gp/'w>; Colour:float<EBC>;}
//    type GrainAddition<[<Measure>] 'w> = {Grain: grain<'w>; Weight:float<'w>}
//    type Addition<[<Measure>] 'w> = {Name:string; Weight:float<'w>;}
//    type yeast<[<Measure>] 't> = {Name:string; Attenuation:float<percentage>; TempRange: float<'t>*float<'t> }
//    type water = {Name:string;} //chemical profile
//
//    type Fermentable<[<Measure>] 'w> =
//    | Grain of GrainAddition<'w>
//    | Adjunct of AdjunctAddition<'w>
//
//    type Ingredient =
//    | Hop of HopAddition<g>
//    | Addition of adjunct<g>
//    | Fermentable of Fermentable<kg>
//
//    

    type Alpha = Alpha of float<percentage>
    //type Colour = Colour of float<EBC>
    type Potential = Potential of float<gp/kg>
    type Weight = Weight of float<g>
    type Time = Time of float<minute>
    type TempRange<[<Measure>] 't> = TempRange of float<'t> * float<'t>

    type Yeast<[<Measure>] 't> = { Name:string; Attenuation:float<percentage>; TempRange:TempRange<'t> }

    type Fermentable =
    | Grain of Potential * Colour:float<EBC> * Name:string
    | Adjunct of Potential * Name:string

    type Hop = Hop of Alpha:Alpha * Name:string 
    type Additive = Additive of Name:string

    
    type HopAddition = { Hop:Hop; Weight:Weight; Time:Time; Type:HopType}
    type FermentableAddition = Fermentable * Weight 
    type AdditiveAddition = Additive * Weight

    type Ingredient =
    | HopAddition of HopAddition
    | FermentableAddition of FermentableAddition
    | AdditiveAddition of AdditiveAddition
