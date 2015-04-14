namespace Models

open Units
open Calculations
open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

type HopType =
|Pellet
|Leaf
|Extract

type HopAddition =
|Bittering
|Flavour
|Aroma
|DryHop

type hop<[<Measure>] 'w> = {Name:string; Weight:float<'w>; Alpha:float<percentage>; Time:float; Type:HopType; }
type adjunct<[<Measure>] 'w> = {Name:string; Weight:float<'w>; Description:string }
type grain<[<Measure>] 'w> = {Name:string; Weight:float<'w>; Potential:float<gp/'w>; Colour:float<EBC>}
type yeast<[<Measure>] 't> = {Name:string; Attenuation:float<percentage>; TempRange: float<'t>*float<'t> }
type water = {Name:string;} //chemical profile

//type Ingredient =
//| Hop of hop
//| Adjunct of adjunct
//| Grain of grain

