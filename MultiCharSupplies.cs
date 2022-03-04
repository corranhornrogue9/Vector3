using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiCharSupplies : MonoBehaviour
{
    public float Mass = 100;
    public float MaxMass = 100;
    public float Energy = 100;
    public float MaxEnergy = 100;
    public float BioMass = 100;
    public float MaxBioMass = 100;
    public float ReactionMass = 100;
    public float MaxReactionMass = 100;

    public bool ReactMass = false;
    public bool CreateBiomass = false;
    public bool CreateReactionMass = false;

    public float ConversionPower = 5;

    public float ReactToEnergy = 0.01f;
    public float EnergyToBio = 1;
    public float MassToBio = 3;

    public float EnergyCostJet = 1;
    public float MassCostJet = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ReactMass && Energy < MaxEnergy)
        {
            var reactAmmount = Mathf.Min(MaxEnergy - Energy, ConversionPower * Time.deltaTime);

            var energy = DrawReactionMass(reactAmmount * ReactToEnergy) / ReactToEnergy;

            SupplyEnergy(energy);

        }

        if (CreateBiomass && BioMass < MaxBioMass)
        {
            var bioMassAmount = Mathf.Min(MaxBioMass - BioMass, ConversionPower * Time.deltaTime);

            var energy = DrawEnergy(bioMassAmount * EnergyToBio) / EnergyToBio;
            var mass = DrawMass(bioMassAmount * MassToBio) / MassToBio;

            var extraEnergy =  (energy * EnergyToBio) -  (mass * MassToBio);

            if(extraEnergy > 0)
            {
                SupplyEnergy(extraEnergy);
            }
            else if(extraEnergy < 0)
            {
                mass -= extraEnergy;
            }

            SupplyBioMass(mass);
        }
    }

    public float DrawReactionMass(float amount)
    {
        var retval = Mathf.Min(amount, ReactionMass);
        ReactionMass -= retval;
        return retval;
    }

    public float DrawEnergy(float amount)
    {
        var retval = Mathf.Min(amount, Energy);
        Energy -= retval;
        return retval;
    }

    public float DrawBioMass(float amount)
    {
        var retval = Mathf.Min(amount, BioMass);
        BioMass -= retval;
        return retval;
    }

    public float DrawMass(float amount)
    {
        var retval = Mathf.Min(amount, Mass);
        Mass -= retval;
        return retval;
    }

    public float SupplyEnergy(float amount)
    {
        Energy += amount;
        var retVal = Mathf.Max(0, Energy - MaxEnergy);
        Energy -= retVal;
        return retVal;
    }

    public float SupplyMass(float amount)
    {
        Mass += amount;
        var retVal = Mathf.Max(0, Mass - MaxMass);
        Mass -= retVal;
        return retVal;
    }

    public float SupplyBioMass(float amount)
    {
        BioMass += amount;
        var retVal = Mathf.Max(0, BioMass - MaxBioMass);
        BioMass -= retVal;
        return retVal;
    }

    public float SupplyReactionMass(float amount)
    {
        ReactionMass += amount;
        var retVal = Mathf.Max(0, ReactionMass - MaxReactionMass);
        ReactionMass -= retVal;
        return retVal;
    }

    public float DrawJetSupplies( float request)
    {
        if(request < 0)
        {
            request = request * -1;
        }
        var energy = DrawEnergy(request * EnergyCostJet) / EnergyCostJet;
        var mass = DrawMass(request * MassCostJet) / MassCostJet;

        var extraEnergy = (energy * EnergyCostJet) - (mass * MassCostJet);

        if (extraEnergy > 0)
        {
            SupplyEnergy(extraEnergy);
        }
        else if (extraEnergy < 0)
        {
            mass -= extraEnergy;
        }

        return mass;
    }
}
