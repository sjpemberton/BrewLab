///Namespace containing Units of Measure for use with Brewing Calculations
namespace Units

open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

(* Define custom units of measure *)

//Time
///Days
[<Measure>]type day
///Days
[<Measure>]type hour
///Days
[<Measure>]type minute
///Days
[<Measure>]type second

///Percentage
[<Measure>]type percentage

//Temp
///Degrees Celsius
[<Measure>] type degC
///Degrees Fahrenheit
[<Measure>] type degF

//Volumes
///Litres - m^3/1000
[<Measure>] type L 

///Imperial Gallon
[<Measure>] type Gal

///US Gallon
[<Measure>] type usGal

//weights
///Grams
[<Measure>] type g

///Millgrams
[<Measure>] type mg

///Pound
[<Measure>] type lb

///Ounces
[<Measure>] type oz

//Gravity
///Specific Gravity - Ratio of density compared to water measured in Kg/L
[<Measure>] type sg = kg / L  //gp / usGal

///Gravity Point - A Simplified brewing unit for amount of sugar dissolved in solution
[<Measure>] type gp

///The number of gravity points per Gallon(US
[<Measure>] type ppg = gp / usGal

///Potential Gravity Points The number of Gravity points in a lb of malt
[<Measure>] type pgp = gp / lb

///Potential Gravity Points - The number of Gravity points in a Kg of malt
[<Measure>] type pgpkg = gp / kg

///Hot Water Extract. Points per Litre per Kilo
[<Measure>] type hwe = gp / L

//Alcohol
/// Alcohol By Volume
[<Measure>] type ABV 

//Bitterness
[<Measure>] type IBU = mg /L

///Alpha Acid Units
[<Measure>] type AAU = g/L

///Colour
///SRM - Standard Reference Method
[<Measure>] type SRM

///European Brewing Convention
[<Measure>] type EBC

///Degrees Lovibond
[<Measure>] type degL

//Malt Colour Unit
[<Measure>] type MCU = degL lb/usGal

///Carbonation
///CO2 - measured in g per L
[<Measure>] type CO2 = g/L

///Simple Conversion constants and functions 
module Conversions =

    let sucroseBasePoints = 46.0<ppg>
    let litresPerGal = 4.54609<L/Gal>
    let litresPerUsGallon = 3.78541<L/usGal>
    let degreesFperC = 1.8<degF/degC>
    let hweInPpg = 8.3454<hwe/ppg>
    let potentialPerKiloInPound = 8.3454<pgpkg/pgp>
    let ouncesPerPound = 16<oz/lb>
    let poundPerKg = 2.20462<lb/kg>
    let srmPerEbc = 1.97<EBC/SRM>

    let ToFahrenheit degreesC = degreesC * degreesFperC + 32.0<degF>
    let ToCelsius degreesF = (degreesF - 32.0<degF>) / degreesFperC 
    let ToPPG (hwe:float<hwe>) = hwe / hweInPpg
    let ToHwe (ppg:float<ppg>) = ppg * hweInPpg
    let ToPGP (pgpkg:float<pgpkg>) = pgpkg / potentialPerKiloInPound
    let ToPGPKg (pgp:float<pgp>) = pgp * potentialPerKiloInPound
    let ToPound (kg:float<kg>) = poundPerKg * kg
    let ToKilograms (lb:float<lb>) = lb / poundPerKg
    let ToLitres (gallons:float<usGal>) = gallons * litresPerUsGallon
    let ToUsGallons (litres:float<L>) = litres / litresPerUsGallon
    let ToEBC (srm:float<SRM>) = srm * srmPerEbc
    let ToSRM (ebc:float<EBC>) = ebc / srmPerEbc