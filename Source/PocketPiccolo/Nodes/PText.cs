/* 
 * Copyright (c) 2003-2004, University of Maryland
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided
 * that the following conditions are met:
 * 
 *		Redistributions of source code must retain the above copyright notice, this list of conditions
 *		and the following disclaimer.
 * 
 *		Redistributions in binary form must reproduce the above copyright notice, this list of conditions
 *		and the following disclaimer in the documentation and/or other materials provided with the
 *		distribution.
 * 
 *		Neither the name of the University of Maryland nor the names of its contributors may be used to
 *		endorse or promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 * TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * Piccolo was written at the Human-Computer Interaction Laboratory www.cs.umd.edu/hcil by Jesse Grosjean
 * and ported to C# by Aaron Clamage under the supervision of Ben Bederson.  The Piccolo website is
 * www.cs.umd.edu/hcil/piccolo.
 */

using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

using UMD.HCIL.PocketPiccolo;
using UMD.HCIL.PocketPiccolo.Util;

namespace UMD.HCIL.PocketPiccolo.Nodes {
	/// <summary>
	/// <b>PText</b> is a multi-line text node.  The text will wrap based on the width
	/// of the node's bounds.
	/// </summary>
	//[Serializable]
	public class PText : PNode { //, ISerializable {

		#region Fields
		/// <summary>
		/// The default font to use when rendering this PText node.
		/// </summary>
		public static Font DEFAULT_FONT = new Font("Arial", 12, FontStyle.Regular);

		private Graphics GRAPHICS = Graphics.FromImage(new Bitmap(1, 1));  // was static
		private String text;
		[NonSerialized] private Brush textBrush;
		private Font font;
//		[NonSerialized] private StringFormat stringFormat = new StringFormat();
		private bool constrainHeightToTextHeight = true;
		private bool constrainWidthToTextWidth = true;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new PText with an empty string.
		/// </summary>
		public PText() {
			textBrush = new SolidBrush(Color.Black); //Brushes.Black;
		}

		/// <summary>
		/// Constructs a new PText with the given string.
		/// </summary>
		/// <param name="aText">The desired text string for this PText.</param>
		public PText(String aText) : this() {
			Text = aText;
		}
		#endregion

		#region Basic
		//****************************************************************
		// Basic - Methods for manipulating the underlying text.
		//****************************************************************

		/// <summary>
		/// Gets or sets a value indicating whether this node changes its width to fit
		/// the width of its text.
		/// </summary>
		/// <value>
		/// True if this node changes its width to fit its text width; otherwise, false.
		/// </value>
		public virtual bool ConstrainWidthToTextWidth {
			get { return constrainWidthToTextWidth; }
			set {
				constrainWidthToTextWidth = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this node changes its height to fit
		/// the height of its text.
		/// </summary>
		/// <value>
		/// True if this node changes its height to fit its text height; otherwise, false.
		/// </value>
		public virtual bool ConstrainHeightToTextHeight {
			get { return constrainHeightToTextHeight; }
			set {
				constrainHeightToTextHeight = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/// <summary>
		/// Gets or sets the text for this node.
		/// </summary>
		/// <value>This node's text.</value>
		/// <remarks>
		/// The text will be broken up into multiple lines based on the size of the text
		/// and the bounds width of this node.
		/// </remarks>
		public virtual String Text {
			get { return text; }
			set {
				text = value;

				InvalidatePaint();
				RecomputeBounds();
			}
		}

		/*
		/// <summary>
		/// Gets or sets a value specifiying the alignment to use when rendering this
		/// node's text.
		/// </summary>
		/// <value>The alignment to use when rendering this node's text.</value>
		public virtual StringAlignment TextAlignment {
			get { return stringFormat.Alignment; }
			set {
				stringFormat.Alignment = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}
		*/

		/// <summary>
		/// Gets or sets the brush to use when rendering this node's text.
		/// </summary>
		/// <value>The brush to use when rendering this node's text.</value>
		public virtual Brush TextBrush {
			get { return textBrush; }
			set {
				textBrush = value; 
				InvalidatePaint();
			}
		}

		/// <summary>
		/// Gets or sets the font to use when rendering this node's text.
		/// </summary>
		/// <value>The font to use when rendering this node's text.</value>
		public virtual Font Font {
			get {
				if (font == null) {
					font = DEFAULT_FONT;
				}
				return font;
			}
			set {
				font = value;
				InvalidatePaint();
				RecomputeBounds();
			}
		}
		#endregion

		#region Painting
		//****************************************************************
		// Painting - Methods for painting a PText.
		//****************************************************************

		/// <summary>
		/// Overridden.  See <see cref="PNode.Paint">PNode.Paint</see>.
		/// </summary>
		protected override void Paint(UMD.HCIL.PocketPiccolo.Util.PPaintContext paintContext) {
			base.Paint(paintContext);

			if (text != null && textBrush != null && font != null) {
				Graphics2D g = paintContext.Graphics;

				float renderedFontSize = font.Size * paintContext.Scale;
					//font.SizeInPoints * paintContext.Scale;
				if (renderedFontSize < PUtil.GreekThreshold) {
					
					// .NET bug: DrawString throws a generic gdi+ exception when
					// the scaled font size is very small.  So, we will render
					// the text as a simple rectangle for small fonts
					g.FillRectangle(textBrush, Bounds);
				}
				else if (renderedFontSize < PUtil.MaxFontSize) {
					g.DrawString(text, font, textBrush, Bounds); //, stringFormat);
				}
			}
		}
		#endregion

		#region Bounds
		//****************************************************************
		// Bounds - Methods for manipulating the bounds of a PText.
		//****************************************************************

		/// <summary>
		/// Overridden.  See <see cref="PNode.InternalUpdateBounds">PNode.InternalUpdateBounds</see>.
		/// </summary>
		protected override void InternalUpdateBounds(float x, float y, float width, float height) {
			RecomputeBounds();
		}

		/// <summary>
		/// Override this method to change the way bounds are computed. For example
		/// this is where you can control how lines are wrapped.
		/// </summary>
		public virtual void RecomputeBounds() {
			if (text != null && (ConstrainWidthToTextWidth || ConstrainHeightToTextHeight)) {
				float textWidth;
				float textHeight;
				if (ConstrainWidthToTextWidth) {
					textWidth = GRAPHICS.MeasureString(Text, Font).Width;
					textHeight = GRAPHICS.MeasureString(Text, Font).Height;
				}
				else {
					textWidth = Width;
					textHeight = PUtil.MeasureString(GRAPHICS, Text, Font, (int)textWidth).Height;
					//textHeight = GRAPHICS.MeasureString(Text, Font, (int)textWidth).Height;
				}

				float newWidth = Width;
				float newHeight = Height;
				if (ConstrainWidthToTextWidth) newWidth = textWidth;
				if (ConstrainHeightToTextHeight) newHeight = textHeight;

				base.SetBounds(X, Y, newWidth, newHeight);
			}
		}
		#endregion

		#region Serialization
		//****************************************************************
		// Serialization - Nodes conditionally serialize their parent.
		// This means that only the parents that were unconditionally
		// (using GetObjectData) serialized by someone else will be restored
		// when the node is deserialized.
		//****************************************************************
		/*
		/// <summary>
		/// Read this this PText and all its children from the given SerializationInfo.
		/// </summary>
		/// <param name="info">The SerializationInfo to read from.</param>
		/// <param name="context">
		/// The StreamingContext of this serialization operation.
		/// </param>
		/// <remarks>
		/// This constructor is required for Deserialization.
		/// </remarks>
		protected PText(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			textBrush = PUtil.ReadBrush(info, "textbrush");
			TextAlignment = (StringAlignment)info.GetValue("alignment", typeof(int));
		}

		/// <summary>
		/// Write this PText and all of its descendent nodes to the given SerializationInfo.
		/// </summary>
		/// <param name="info">The SerializationInfo to write to.</param>
		/// <param name="context">The streaming context of this serialization operation.</param>
		/// <remarks>
		/// This node's parent is written out conditionally, that is it will only be written out
		/// if someone else writes it out unconditionally.
		/// </remarks>
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData (info, context);

			PUtil.WriteBrush(textBrush, "textbrush", info);
			info.AddValue("alignment", stringFormat.Alignment);
		}
		*/
		#endregion

		#region Debugging
		//****************************************************************
		// Debugging - Methods for debugging.
		//****************************************************************

		/// <summary>
		/// Overridden.  Gets a string representing the state of this node.
		/// </summary>
		/// <value>A string representation of this node's state.</value>
		/// <remarks>
		/// This property is intended to be used only for debugging purposes, and the content
		/// and format of the returned string may vary between implementations. The returned
		/// string may be empty but may not be <c>null</c>.
		/// </remarks>
		protected override String ParamString {
			get {
				StringBuilder result = new StringBuilder();

				result.Append("text=" + (text == null ? "null" : text));
				result.Append(",font=" + (font == null ? "null" : font.ToString()));
				result.Append(',');
				result.Append(base.ParamString);

				return result.ToString();
			}
		}
		#endregion
	}
}
