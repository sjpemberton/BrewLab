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

//Temp fixed list of grain
    module Grain =

        let Grains = [{ Name = "Maris Otter"
                        Weight = 0.0<kg>
                        Potential = 37.0<pgpkg>
                        Colour = 4.0<EBC> };
                      { Name = "Cara Amber"
                        Weight = 0.0<kg>
                        Potential = 35.0<pgpkg>
                        Colour = 20.0<EBC> };
                      { Name = "Cara Pils"
                        Weight = 0.0<kg>
                        Potential = 32.0<pgpkg>
                        Colour = 10.0<EBC> }]