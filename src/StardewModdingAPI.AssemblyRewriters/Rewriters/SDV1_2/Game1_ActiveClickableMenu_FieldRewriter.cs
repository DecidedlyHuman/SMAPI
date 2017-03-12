using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;
using Mono.Cecil.Cil;
using StardewModdingAPI.AssemblyRewriters.Framework;
using StardewValley;

namespace StardewModdingAPI.AssemblyRewriters.Rewriters.SDV1_2
{
    /// <summary>Rewrites field references to <see cref="Game1.activeClickableMenu"/>.</summary>
    /// <remarks>Stardew Valley changed the <see cref="Game1.activeClickableMenu"/> field to a property, which broke many mods that reference it.</remarks>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "This class is not meant to be used directly, and is deliberately named to make it easier to know what it changes at a glance.")]
    public class Game1_ActiveClickableMenu_FieldRewriter : BaseFieldRewriter
    {
        /*********
        ** Accessors
        *********/
        /// <summary>A brief noun phrase indicating what the instruction finder matches.</summary>
        public override string NounPhrase { get; } = $"{nameof(Game1)}.{nameof(Game1.activeClickableMenu)} field";


        /*********
        ** Protected methods
        *********/
        /// <summary>Get whether a field reference should be rewritten.</summary>
        /// <param name="instruction">The IL instruction.</param>
        /// <param name="fieldRef">The field reference.</param>
        /// <param name="platformChanged">Whether the mod was compiled on a different platform.</param>
        protected override bool IsMatch(Instruction instruction, FieldReference fieldRef, bool platformChanged)
        {
            return
                this.IsStaticField(instruction)
                && fieldRef.DeclaringType.FullName == typeof(Game1).FullName
                && fieldRef.Name == nameof(Game1.activeClickableMenu);
        }

        /// <summary>Rewrite a method for compatibility.</summary>
        /// <param name="module">The module being rewritten.</param>
        /// <param name="cil">The CIL rewriter.</param>
        /// <param name="instruction">The instruction which references the field.</param>
        /// <param name="fieldRef">The field reference invoked by the <paramref name="instruction"/>.</param>
        /// <param name="assemblyMap">Metadata for mapping assemblies to the current platform.</param>
        protected override void Rewrite(ModuleDefinition module, ILProcessor cil, Instruction instruction, FieldReference fieldRef, PlatformAssemblyMap assemblyMap)
        {
            string methodPrefix = instruction.OpCode == OpCodes.Ldsfld ? "get" : "set";
            MethodReference propertyRef = module.Import(typeof(Game1).GetMethod($"{methodPrefix}_{nameof(Game1.activeClickableMenu)}"));
            cil.Replace(instruction, cil.Create(OpCodes.Call, propertyRef));
        }
    }
}
