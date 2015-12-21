/// Brewing Calculations for all stages of the brewing process
///
/// Calculations
///
module Calculations 

open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols
open LanguagePrimitives
open Units
open Conversions


(** Types for use with the calculations *)

///A particular amount of a malt with a specified potential
//type Malt<[<Measure>] 'u> = 
//    {Weight:float<'u>; Potential:float<gp/'u>;}

(**
Generic/Common Functions 
*)

///Converts a Specific Gravity into brewing Specific Gravity Points
let ToGravPoints (gravity:float<sg>) =
    (float gravity - 1.0) * 1000.0<gp>

///Converts the given GP into Specific Gravity
let ToGravity (gravityPoints:float<gp>) =
    ((float gravityPoints / 1000.0) + 1.0) * 1.0<sg>

///Estimated FG - Estimates the final gravity of a beer based on the starting gravity points and the yeast attenuation
let EstimateFinalGravity originalGravity (attenuation:float<percentage>) =
    let points = originalGravity |> ToGravPoints
    let pointsAfterFermentation = (points - (points * (float attenuation / 100.0))) 
    pointsAfterFermentation|> ToGravity

///Yeast attenuation - The difference in specific gravity between OG and FG
let YeastAttenuation (originalGravity:float<sg>) (finalGravity:float<sg>) =
    ((originalGravity - finalGravity) / (originalGravity - 1.0<sg>)) * 100.0<percentage>

///Estimated ABV
let AlcoholByVolume (originalGravity:float<sg>) (finalGravity:float<sg>) : float<ABV> = 
    FloatWithMeasure ((1.05 / 0.79) * ((originalGravity - finalGravity) / finalGravity)) * 100.0

let ABVsimple (og:float<sg>) (fg:float<sg>) : float<ABV> = 
    FloatWithMeasure 131.0 * (og - fg)

///Calculates required Grain in weight from the target gravity points and effective malt potential (in relation to a given weight).
let GrainRequired<[<Measure>]'u> (gravityPoints:float<gp>) (effectivePotential:float<gp/'u>) =
    gravityPoints / effectivePotential

///Mash Efficiency from points - Pre-boil points / Potential max points
let Efficiency (potential:float<gp>) (actual:float<gp>) = 
    (actual / potential) * 1.0<percentage>





(** 
Functions working on US/Imperial Units. EG: PPG, lb, oz, usGallon 
*)

///Required grain in pound based on a malt potential in %, mash efficiency and total gravity points
//let RequiredGrainInPounds (gravityPoints:float<gp>) (potential:float<percentage>) (efficiency:float<percentage>) =
//    GrainRequired<lb> gravityPoints (float((potential / 100.0) * (efficiency / 100.0) * 46.0<ppg>) * 1.0<pgp>)

(**
Functions that work on British/Metric units - HWE, L, Kg
*)

///Converts a points per Litre (gp/L - HWE) and volume into total gravity points in that volume
let TotalGravityPoints (potential:float<gp / L>) (vol : float<L>) =  
    potential * vol

///Calculates the total specific gravity points needed to achieve the given volume at the given specific gravity - not taking into account malt or weight
let RequiredPoints (targetGravity:float<sg>) (vol:float<L>) = 
    TotalGravityPoints ((targetGravity |> ToGravPoints) / 1.0<L>) vol

///The maximum potential points (in HWE (gp/L)) for a given weight of grain with the given extract potential, divided by the target volume
let MaxPotentialPoints (grainPotential:float<pgpkg>) (grain:float<kg>) (vol:float<L>) :float<hwe> = 
    (grainPotential * grain) / vol

///Efficiency taking into account losses during process  
///Can be used to measure efficiency at various stages. Just substitute in the actual points and Vol at a particular time. eg pre or post boil
let CalculateBrewHouseEfficiency (potential:float<hwe>) (actual:float<hwe>) (targetVol:float<L>) (actualVol:float<L>) =
    ((actual * actualVol) / (potential * targetVol)) * 1.0<percentage>

let EstimateGravityPoints (grainPotential:float<pgpkg>) (grain:float<kg>) (vol:float<L>) (efficiency:float<percentage>) :float<gp>=
    ((grainPotential * grain * (float efficiency / 100.0)) / vol) * 1.0<L>

///The estimated gravity of wort created from an amount of grain in lb with the given ppg, at a particular efficiency and for a target volume
let EstimateGravity  (grainPotential:float<pgpkg>) (grain:float<kg>) (vol:float<L>) (efficiency:float<percentage>) =
    EstimateGravityPoints grainPotential grain vol efficiency
    |> ToGravity


///Required grain in kilo based on a malt potential in HWE, mash efficiency and total gravity points
let RequiredGrainInKilo (gravityPoints:float<gp>) (potential:float<hwe>) (efficiency:float<percentage>)  =
    GrainRequired<kg> gravityPoints (float(potential * (efficiency / 100.0) * (46.0<ppg> |> ToHwe)) * 1.0<gp/kg>)

///Efficiency Calculation using Litres and HWE
let CalculateBrewHouseEfficiencyLitres (potential:float<hwe>) (actual:float<hwe>) (targetVol:float<L>) (actualVol:float<L>) =
    ((actual * actualVol) / (potential * targetVol)) * 1.0<percentage>

//Mash Efficiency - alternate - using a 'standard' 37.0 ppg. based on lager malt maximum. For use when individual grain potential is not known
//let alternativeEfficiency (og:float<sg>) (vol : float<'u>) (grain:float<'v>) :float<percentage> =
//    let ppg = totalGravityPoints og vol
//    ((float ppg / float grain) / 37.0) * 100.0<percentage>
//

///Bitterness
///Calculates Aplha Acid Units
let AplhaAcidUnits (weight:float<g>) (alpha:float<percentage>) :float<AAU> =
    FloatWithMeasure (float(weight * alpha))

///Calculates IBU (mg/L) from AAUs
let IBUsFromAAU (utilisation:float<percentage>) (aau:float<AAU>)  (volume:float<L>)  :float<IBU> = 
    FloatWithMeasure (float(aau * utilisation * 10.0 / volume))

///Calculates IBUS (mg/L) from AA% weight
let EstimateIBUs (utilisation:float<percentage>) (alpha:float<percentage>) (weight:float<g>) (volume:float<L>) :float<IBU> =
    IBUsFromAAU utilisation (AplhaAcidUnits weight alpha) volume

//The Tinseth Equation for hop utilisation. Time in minutes
let EstimateHopUtilisation (gravity:float<sg>) (time:float) :float<percentage> = 
    (1.65 * 0.000125 ** (float gravity - 1.0)) * ((1.0 - 2.71828 ** (-0.04 * time)) / 4.15)
    |> FloatWithMeasure


///Colour
///Calculates an MCU value from grain weight, colour in degrees lovibond and volume
let Mcu (weight:float<kg>) (colour:float<degL>) (volume:float<L>) :float<MCU> =
    (weight |> ToPound) * colour / (volume |> ToUsGallons)

///Returns the SRM value from an MCU value
let SrmFromMcu (mcu:float<MCU>) :float<SRM> = 
    FloatWithMeasure 1.4922 * (float mcu ** 0.6859)

let GrainEBC (weight:float<kg>) (colour:float<EBC>) = 
    float weight * colour

let TotalEBC (efficiency:float<percentage>) (volume:float<L>) (additions :float<EBC> list) :float<EBC> = 
     FloatWithMeasure (float (List.sum additions * 10.0 * (efficiency / 100.0) / volume))

//Taken from Malt.io
///Convert an SRM value into rough RGB 
let SrmToRgb (srm:float<SRM>) = 
    (System.Math.Round(min 255.0 (max 0.0 (255.0 * 0.975 ** float srm))),
     System.Math.Round(min 255.0 (max 0.0 (245.0 * 0.88 ** float srm))),
     System.Math.Round(min 255.0 (max 0.0 (220.0 * 0.7 ** float srm))))



//Priming Sugar 
///Calculates the final CO2 level in a beer - co2PerGram is CO2 created per gram of sugar
let FinalCo2 co2PerGram (currentCarb : float<CO2>) (sugar : float<g>) (volume : float<L>) =
    currentCarb + co2PerGram * sugar / volume

///Carbonation added by Corn sugar (Glucose Monohydrate)
let Co2CornSugar = FinalCo2 (0.5 * 0.91) 
///Carbonation with table sugar
let Co2TableSugar = FinalCo2 (0.5)
///Carbonation with Dried Malt Extract (DME)
let Co2Dme = FinalCo2 (0.5 * 0.82 * 0.80)

///Calculates the amount of sugar required to create the target amount of CO2 - co2PerGram is CO2 created per gram of sugar
let PrimingSugarAmount co2PerGram  (currentCarb : float<CO2>) (targetCarb : float<CO2>) (volume : float<L>) :float<g> =
    volume * (targetCarb - currentCarb) / co2PerGram

///Amount of Corn sugar to produce target CO2
let CalculateRequiredCornSugar = PrimingSugarAmount (0.5 * 0.91)
///Amount of Table sugar to produce target CO2
let CalculateRequiredTableSugar = PrimingSugarAmount (0.5)
///Amount of Dried MAlt Extrcat (DME) to produce target CO2
let CalculateRequiredDme = PrimingSugarAmount (0.5 * 0.82 * 0.80)