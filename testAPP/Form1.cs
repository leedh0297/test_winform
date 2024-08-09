﻿using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace testAPP
{
    public partial class Form1 : Form
    {
        private List<string[]> data = new List<string[]>(); // 데이터를 저장할 리스트
        private SortOrder sortOrder = SortOrder.Ascending; // 정렬 순서
        private int sortColumn = -1; // 정렬할 컬럼의 인덱스
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수
        private string connectionString = "Host=192.168.201.151;Username=postgres;Password=12345678;Database=internTest"; // PostgreSQL 연결 문자열

        // 변경된 데이터를 추적하기 위한 리스트
        private List<string[]> addedData = new List<string[]>();
        private List<int> deletedIds = new List<int>();

        public Form1()
        {
            InitializeComponent();

            // 상태 표시줄 초기화
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.Items.Add(statusLabel);
            Controls.Add(statusStrip);

            // ListView의 열을 동적으로 설정
            SetupListViewColumns();

            // 콤보박스 초기화
            SetupComboBox();

            // 데이터베이스에서 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 표시
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // 테이블의 칼럼을 ListView에 동적으로 추가
        private void SetupListViewColumns()
        {
            lv_list.Columns.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            string dataType = reader["data_type"].ToString();

                            // 열 너비를 데이터 유형에 따라 결정 (여기서는 임의로 설정)
                            int columnWidth = dataType == "text" ? 300 : 100;

                            // 열 추가
                            lv_list.Columns.Add(columnName, columnWidth);
                        }
                    }
                }
            }

            lv_list.View = View.Details; // ListView를 상세보기로 설정
        }

        // 데이터베이스에서 데이터 로드
        private void LoadDataFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM book", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        data.Clear(); // 기존 데이터를 지움
                        while (reader.Read())
                        {
                            string[] row = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i].ToString();
                            }
                            data.Add(row); // 새 데이터를 추가
                        }
                    }
                }
            }
            UpdateListView(); // ListView를 업데이트
        }

        // 검색 버튼 클릭 시
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // 삽입 버튼 클릭 시
        private void insert_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text.Trim();
            string writer = tb_writer.Text.Trim();
            string genre = tb_genre.Text.Trim();
            string description = tb_description.Text.Trim();

            // 모든 텍스트박스가 빈칸이면 추가하지 않음
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(writer) &&
                string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(description))
            {
                MessageBox.Show("모든 항목이 빈칸일 수 없습니다.");
                return;
            }

            // 새로운 항목에 임시 ID할당
            string[] newRow = new string[] { Guid.NewGuid().ToString(), title, writer, genre, description };
            addedData.Add(newRow);

            // ListView에 추가
            ListViewItem item = new ListViewItem(newRow);
            lv_list.Items.Add(item);

            // 텍스트박스 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + (data.Count + addedData.Count).ToString();

            // 상태 표시줄 업데이트
            statusLabel.Text = "    항목이 삽입되었습니다.";
        }

        // 리스트 칼럼 클릭 시 정렬
        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                // 기존칼럼을 눌렀을때 정렬 방향을 변경
                sortOrder = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // 새로운 칼럼을 오름차순 정렬
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }

            // list뷰 정렬
            lv_list.ListViewItemSorter = new ListViewItemComparer(e.Column, sortOrder);
        }

        // ListViewItemComparer 클래스: ListView 정렬을 위한 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col; // 정렬할 컬럼
            private SortOrder order; // 정렬 순서

            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal;
                if (col == 0) // 첫 번째 열 (id 열)은 숫자형으로 정렬
                {
                    int id1 = int.Parse(((ListViewItem)x).SubItems[col].Text);
                    int id2 = int.Parse(((ListViewItem)y).SubItems[col].Text);
                    returnVal = id1.CompareTo(id2);
                }
                else
                {
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                               ((ListViewItem)y).SubItems[col].Text);
                }

                if (order == SortOrder.Descending)
                    returnVal *= -1;

                return returnVal;
            }
        }

        // 수정 버튼 클릭 시
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                // 입력된 값을 읽음
                string newTitle = tb_title.Text.Trim();
                string newWriter = tb_writer.Text.Trim();
                string newGenre = tb_genre.Text.Trim();
                string newDescription = tb_description.Text.Trim();

                var selectedItem = lv_list.SelectedItems[0];
                string id = selectedItem.SubItems[0].Text;


                // addedData에서 해당 항목 찾기
                var addedItem = addedData.FirstOrDefault(row => row[0] == id);
                if (addedItem != null)
                {
                    // addedData 항목 수정
                    addedItem[1] = newTitle;
                    addedItem[2] = newWriter;
                    addedItem[3] = newGenre;
                    addedItem[4] = newDescription;
                }
                else
                {
                    // 기존 데이터 수정
                    selectedItem.SubItems[1].Text = newTitle;
                    selectedItem.SubItems[2].Text = newWriter;
                    selectedItem.SubItems[3].Text = newGenre;
                    selectedItem.SubItems[4].Text = newDescription;
                }

                // 상태 표시줄 업데이트
                statusLabel.Text = "    항목이 수정되었습니다.";

            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }
        }

        // 적용 버튼 클릭 시
        private void bt_apply_Click(object sender, EventArgs e)
        {
            // 삭제 항목을 데이터베이스에서 삭제
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // 추가된 항목을 데이터베이스에 삽입
                foreach (var Row in addedData)
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO book (title, writer, genre, description) VALUES (@title, @writer, @genre, @description)", conn))
                    {
                        cmd.Parameters.AddWithValue("title", Row[1]);
                        cmd.Parameters.AddWithValue("writer", Row[2]);
                        cmd.Parameters.AddWithValue("genre", Row[3]);
                        cmd.Parameters.AddWithValue("description", Row[4]);
                        cmd.ExecuteNonQuery();
                    }
                }
                addedData.Clear();

                // 삭제된 항목을 데이터베이스에서 제거
                foreach (var id in deletedIds)
                {
                    using (var cmd = new NpgsqlCommand("DELETE FROM book WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                deletedIds.Clear();
            }

            // 데이터베이스에서 데이터 다시 로드
            LoadDataFromDatabase();

            // 변경 사항 초기화
            addedData.Clear();
            deletedIds.Clear();
            addedItems.Clear();

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 상태 표시줄 업데이트
            statusLabel.Text = "    변경 사항이 데이터베이스에 적용되었습니다.";
        }

        // 삭제 버튼 클릭 시
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                var selectedItem = lv_list.SelectedItems[0];
                string id = selectedItem.SubItems[0].Text;

                // 추가된 항목인지 확인
                var addedItem = addedData.FirstOrDefault(row => row[0] == id);
                if (addedItem != null)
                {
                    // addedData 리스트에서 항목 제거
                    addedData.Remove(addedItem);
                }
                else
                {
                    // 기존 항목 삭제 처리
                    int existingId = int.Parse(id);
                    deletedIds.Add(existingId);
                }

                // ListView에서 항목 삭제
                lv_list.Items.RemoveAt(selectedIndex);

                // 선택된 항목 초기화
                selectedIndex = -1;

                // 텍스트박스 초기화
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";

                // 전체 도서수 업데이트
                total_books.Text = "전체 도서수 : " + (data.Count - deletedIds.Count).ToString();

                // 상태 표시줄 업데이트
                statusLabel.Text = "    항목이 삭제되었습니다.";
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }
        }

        // 새로고침 버튼 클릭 시
        private void bt_refresh_Click(object sender, EventArgs e)
        {
            // 데이터베이스에서 데이터 다시 로드
            LoadDataFromDatabase();

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 콤보박스를 '전체'로 설정
            cb_filter.SelectedIndex = 0;

            // 검색어 텍스트박스 초기화
            tb_search.Text = "";

            // 텍스트박스 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }

        // 콤보박스 초기화
        private void SetupComboBox()
        {
            // 콤보박스 초기화
            cb_filter.Items.Clear();
            cb_filter.Items.Add("전체");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            if (columnName != "id") // 'id' 열은 필터에 포함하지 않음
                            {
                                cb_filter.Items.Add(columnName);
                            }
                        }
                    }
                }
            }

            cb_filter.SelectedIndex = 0; // 기본 설정은 '전체'
        }

        // 콤보박스 선택 항목 변경 시
        private void cb_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // ListView 업데이트
        private void UpdateListView(string filterColumn = "", string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                AddRowToListView(row, filterColumn, searchText);
            }

            foreach (var addedItem in addedItems)
            {
                AddRowToListView(addedItem.Item1, filterColumn, searchText);
            }
        }
        private void AddRowToListView(string[] row, string filterColumn, string searchText)
        {
            bool addRow = true;

            if (!string.IsNullOrEmpty(searchText))
            {
                addRow = row.Any(col => col.ToLower().Contains(searchText));
            }

            if (addRow && !string.IsNullOrEmpty(filterColumn) && filterColumn != "전체")
            {
                int columnIndex = lv_list.Columns.Cast<ColumnHeader>().ToList().FindIndex(c => c.Text == filterColumn);
                if (columnIndex != -1)
                {
                    addRow = row[columnIndex].ToLower().Contains(searchText);
                }
            }

            if (addRow)
            {
                ListViewItem item = new ListViewItem(row);
                lv_list.Items.Add(item);
            }
        }

        // 리스트뷰 선택 항목 변경 시
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                selectedIndex = lv_list.SelectedIndices[0]; // 선택된 항목의 인덱스 저장

                // 선택된 항목의 텍스트박스를 업데이트
                tb_title.Text = lv_list.SelectedItems[0].SubItems[1].Text;
                tb_writer.Text = lv_list.SelectedItems[0].SubItems[2].Text;
                tb_genre.Text = lv_list.SelectedItems[0].SubItems[3].Text;
                tb_description.Text = lv_list.SelectedItems[0].SubItems[4].Text;
            }
        }

        private List<Tuple<string[], ListViewItem>> addedItems = new List<Tuple<string[], ListViewItem>>();

        // 상태 표시줄 추가
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            this.Controls.Add(labelTitle);
            this.Controls.Add(labelWriter);
            this.Controls.Add(labelGenre);
            this.Controls.Add(labelDescription);
        }
    }
}