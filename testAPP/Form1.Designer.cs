namespace testAPP
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.lv_list = new System.Windows.Forms.ListView();
            this.bt_search = new System.Windows.Forms.Button();
            this.bt_insert = new System.Windows.Forms.Button();
            this.bt_delete = new System.Windows.Forms.Button();
            this.tb_search = new System.Windows.Forms.TextBox();
            this.tb_title = new System.Windows.Forms.TextBox();
            this.total_books = new System.Windows.Forms.Label();
            this.tb_genre = new System.Windows.Forms.TextBox();
            this.tb_description = new System.Windows.Forms.TextBox();
            this.tb_writer = new System.Windows.Forms.TextBox();
            this.bt_edit = new System.Windows.Forms.Button();
            this.cb_filter = new System.Windows.Forms.ComboBox();
            this.bt_refresh = new System.Windows.Forms.Button();
            this.bt_apply = new System.Windows.Forms.Button();
            this.title_label = new System.Windows.Forms.Label();
            this.writer_label = new System.Windows.Forms.Label();
            this.genre_label = new System.Windows.Forms.Label();
            this.description_label = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.results_report2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lv_list
            // 
            this.lv_list.FullRowSelect = true;
            this.lv_list.HideSelection = false;
            this.lv_list.Location = new System.Drawing.Point(12, 41);
            this.lv_list.Name = "lv_list";
            this.lv_list.Size = new System.Drawing.Size(760, 214);
            this.lv_list.TabIndex = 8;
            this.lv_list.UseCompatibleStateImageBehavior = false;
            // 
            // bt_search
            // 
            this.bt_search.Location = new System.Drawing.Point(697, 12);
            this.bt_search.Name = "bt_search";
            this.bt_search.Size = new System.Drawing.Size(75, 23);
            this.bt_search.TabIndex = 7;
            this.bt_search.Text = "검색";
            this.bt_search.Click += new System.EventHandler(this.search_Click);
            // 
            // bt_insert
            // 
            this.bt_insert.Location = new System.Drawing.Point(12, 332);
            this.bt_insert.Name = "bt_insert";
            this.bt_insert.Size = new System.Drawing.Size(75, 23);
            this.bt_insert.TabIndex = 13;
            this.bt_insert.Text = "삽입";
            this.bt_insert.UseVisualStyleBackColor = true;
            this.bt_insert.Click += new System.EventHandler(this.insert_Click);
            // 
            // bt_delete
            // 
            this.bt_delete.Location = new System.Drawing.Point(174, 332);
            this.bt_delete.Name = "bt_delete";
            this.bt_delete.Size = new System.Drawing.Size(75, 23);
            this.bt_delete.TabIndex = 15;
            this.bt_delete.Text = "삭제";
            this.bt_delete.UseVisualStyleBackColor = true;
            this.bt_delete.Click += new System.EventHandler(this.bt_delete_Click);
            // 
            // tb_search
            // 
            this.tb_search.Location = new System.Drawing.Point(12, 12);
            this.tb_search.Name = "tb_search";
            this.tb_search.Size = new System.Drawing.Size(573, 21);
            this.tb_search.TabIndex = 5;
            this.tb_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_search_KeyDown);
            // 
            // tb_title
            // 
            this.tb_title.BackColor = System.Drawing.SystemColors.Window;
            this.tb_title.Location = new System.Drawing.Point(12, 303);
            this.tb_title.Name = "tb_title";
            this.tb_title.Size = new System.Drawing.Size(130, 21);
            this.tb_title.TabIndex = 9;
            this.tb_title.Text = "제목";
            // 
            // total_books
            // 
            this.total_books.AutoSize = true;
            this.total_books.Location = new System.Drawing.Point(14, 267);
            this.total_books.Name = "total_books";
            this.total_books.Size = new System.Drawing.Size(65, 12);
            this.total_books.TabIndex = 8;
            this.total_books.Text = "전체도서수";
            // 
            // tb_genre
            // 
            this.tb_genre.Location = new System.Drawing.Point(284, 305);
            this.tb_genre.Name = "tb_genre";
            this.tb_genre.Size = new System.Drawing.Size(130, 21);
            this.tb_genre.TabIndex = 11;
            this.tb_genre.Text = "장르";
            // 
            // tb_description
            // 
            this.tb_description.Location = new System.Drawing.Point(420, 305);
            this.tb_description.Name = "tb_description";
            this.tb_description.Size = new System.Drawing.Size(352, 21);
            this.tb_description.TabIndex = 12;
            this.tb_description.Text = "설명";
            // 
            // tb_writer
            // 
            this.tb_writer.Location = new System.Drawing.Point(148, 305);
            this.tb_writer.Name = "tb_writer";
            this.tb_writer.Size = new System.Drawing.Size(130, 21);
            this.tb_writer.TabIndex = 10;
            this.tb_writer.Text = "작가";
            // 
            // bt_edit
            // 
            this.bt_edit.Location = new System.Drawing.Point(93, 332);
            this.bt_edit.Name = "bt_edit";
            this.bt_edit.Size = new System.Drawing.Size(75, 23);
            this.bt_edit.TabIndex = 14;
            this.bt_edit.Text = "수정";
            this.bt_edit.UseVisualStyleBackColor = true;
            this.bt_edit.Click += new System.EventHandler(this.bt_edit_Click);
            // 
            // cb_filter
            // 
            this.cb_filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_filter.FormattingEnabled = true;
            this.cb_filter.Location = new System.Drawing.Point(591, 15);
            this.cb_filter.Name = "cb_filter";
            this.cb_filter.Size = new System.Drawing.Size(100, 20);
            this.cb_filter.TabIndex = 6;
            // 
            // bt_refresh
            // 
            this.bt_refresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_refresh.FlatAppearance.BorderSize = 0;
            this.bt_refresh.Location = new System.Drawing.Point(697, 332);
            this.bt_refresh.Name = "bt_refresh";
            this.bt_refresh.Size = new System.Drawing.Size(75, 23);
            this.bt_refresh.TabIndex = 16;
            this.bt_refresh.Text = "새로고침";
            this.bt_refresh.UseVisualStyleBackColor = true;
            this.bt_refresh.Click += new System.EventHandler(this.bt_refresh_Click);
            // 
            // bt_apply
            // 
            this.bt_apply.Location = new System.Drawing.Point(697, 401);
            this.bt_apply.Name = "bt_apply";
            this.bt_apply.Size = new System.Drawing.Size(75, 23);
            this.bt_apply.TabIndex = 17;
            this.bt_apply.Text = "저장";
            this.bt_apply.UseVisualStyleBackColor = true;
            this.bt_apply.Click += new System.EventHandler(this.bt_apply_Click);
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Location = new System.Drawing.Point(14, 290);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(29, 12);
            this.title_label.TabIndex = 19;
            this.title_label.Text = "Title";
            // 
            // writer_label
            // 
            this.writer_label.AutoSize = true;
            this.writer_label.Location = new System.Drawing.Point(148, 290);
            this.writer_label.Name = "writer_label";
            this.writer_label.Size = new System.Drawing.Size(36, 12);
            this.writer_label.TabIndex = 20;
            this.writer_label.Text = "Writer";
            // 
            // genre_label
            // 
            this.genre_label.AutoSize = true;
            this.genre_label.Location = new System.Drawing.Point(284, 290);
            this.genre_label.Name = "genre_label";
            this.genre_label.Size = new System.Drawing.Size(39, 12);
            this.genre_label.TabIndex = 21;
            this.genre_label.Text = "Genre";
            // 
            // description_label
            // 
            this.description_label.AutoSize = true;
            this.description_label.Location = new System.Drawing.Point(418, 290);
            this.description_label.Name = "description_label";
            this.description_label.Size = new System.Drawing.Size(68, 12);
            this.description_label.TabIndex = 22;
            this.description_label.Text = "Description";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.results_report2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 427);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // results_report2
            // 
            this.results_report2.Name = "results_report2";
            this.results_report2.Size = new System.Drawing.Size(31, 17);
            this.results_report2.Text = "결과";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 449);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.description_label);
            this.Controls.Add(this.genre_label);
            this.Controls.Add(this.writer_label);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.bt_apply);
            this.Controls.Add(this.bt_refresh);
            this.Controls.Add(this.cb_filter);
            this.Controls.Add(this.bt_edit);
            this.Controls.Add(this.tb_writer);
            this.Controls.Add(this.tb_description);
            this.Controls.Add(this.tb_genre);
            this.Controls.Add(this.total_books);
            this.Controls.Add(this.tb_title);
            this.Controls.Add(this.tb_search);
            this.Controls.Add(this.bt_delete);
            this.Controls.Add(this.bt_insert);
            this.Controls.Add(this.bt_search);
            this.Controls.Add(this.lv_list);
            this.Name = "Form1";
            this.Text = "도서검색";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lv_list;
        private System.Windows.Forms.Button bt_search;
        private System.Windows.Forms.Button bt_insert;
        private System.Windows.Forms.Button bt_delete;
        private System.Windows.Forms.TextBox tb_search;
        private System.Windows.Forms.TextBox tb_title;
        private System.Windows.Forms.Label total_books;
        private System.Windows.Forms.TextBox tb_genre;
        private System.Windows.Forms.TextBox tb_description;
        private System.Windows.Forms.TextBox tb_writer;
        private System.Windows.Forms.Button bt_edit;
        private System.Windows.Forms.ComboBox cb_filter;
        private System.Windows.Forms.Button bt_refresh;
        private System.Windows.Forms.Button bt_apply;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label writer_label;
        private System.Windows.Forms.Label genre_label;
        private System.Windows.Forms.Label description_label;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel results_report2;
    }
}