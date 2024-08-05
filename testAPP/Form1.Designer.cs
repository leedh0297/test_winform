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
            this.SuspendLayout();
            // 
            // lv_list
            // 
            this.lv_list.FullRowSelect = true;
            this.lv_list.HideSelection = false;
            this.lv_list.Location = new System.Drawing.Point(12, 41);
            this.lv_list.Name = "lv_list";
            this.lv_list.Size = new System.Drawing.Size(760, 214);
            this.lv_list.TabIndex = 5;
            this.lv_list.UseCompatibleStateImageBehavior = false;
            this.lv_list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged_1);
            this.lv_list.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ly_click);
            this.lv_list.ImeModeChanged += new System.EventHandler(this.list_ImeModeChanged);
            // 
            // bt_search
            // 
            this.bt_search.Location = new System.Drawing.Point(697, 12);
            this.bt_search.Name = "bt_search";
            this.bt_search.Size = new System.Drawing.Size(75, 23);
            this.bt_search.TabIndex = 4;
            this.bt_search.Text = "검색";
            this.bt_search.Click += new System.EventHandler(this.search_Click);
            // 
            // bt_insert
            // 
            this.bt_insert.Location = new System.Drawing.Point(12, 332);
            this.bt_insert.Name = "bt_insert";
            this.bt_insert.Size = new System.Drawing.Size(75, 23);
            this.bt_insert.TabIndex = 2;
            this.bt_insert.Text = "삽입";
            this.bt_insert.UseVisualStyleBackColor = true;
            this.bt_insert.Click += new System.EventHandler(this.insert_Click);
            // 
            // bt_delete
            // 
            this.bt_delete.Location = new System.Drawing.Point(174, 332);
            this.bt_delete.Name = "bt_delete";
            this.bt_delete.Size = new System.Drawing.Size(75, 23);
            this.bt_delete.TabIndex = 3;
            this.bt_delete.Text = "삭제";
            this.bt_delete.UseVisualStyleBackColor = true;
            // 
            // tb_search
            // 
            this.tb_search.Location = new System.Drawing.Point(12, 12);
            this.tb_search.Name = "tb_search";
            this.tb_search.Size = new System.Drawing.Size(679, 21);
            this.tb_search.TabIndex = 6;
            // 
            // tb_title
            // 
            this.tb_title.Location = new System.Drawing.Point(12, 305);
            this.tb_title.Name = "tb_title";
            this.tb_title.Size = new System.Drawing.Size(130, 21);
            this.tb_title.TabIndex = 7;
            this.tb_title.Text = "제목";
            // 
            // total_books
            // 
            this.total_books.AutoSize = true;
            this.total_books.Location = new System.Drawing.Point(23, 276);
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
            this.tb_genre.TabIndex = 9;
            this.tb_genre.Text = "장르";
            // 
            // tb_description
            // 
            this.tb_description.Location = new System.Drawing.Point(420, 305);
            this.tb_description.Name = "tb_description";
            this.tb_description.Size = new System.Drawing.Size(271, 21);
            this.tb_description.TabIndex = 11;
            this.tb_description.Text = "내용";
            // 
            // tb_writer
            // 
            this.tb_writer.Location = new System.Drawing.Point(148, 305);
            this.tb_writer.Name = "tb_writer";
            this.tb_writer.Size = new System.Drawing.Size(130, 21);
            this.tb_writer.TabIndex = 12;
            this.tb_writer.Text = "작가";
            // 
            // bt_edit
            // 
            this.bt_edit.Location = new System.Drawing.Point(93, 332);
            this.bt_edit.Name = "bt_edit";
            this.bt_edit.Size = new System.Drawing.Size(75, 23);
            this.bt_edit.TabIndex = 13;
            this.bt_edit.Text = "수정";
            this.bt_edit.UseVisualStyleBackColor = true;
            this.bt_edit.Click += new System.EventHandler(this.bt_edit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 449);
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
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
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
    }
}

