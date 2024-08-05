using Npgsql.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testAPP
{
    public partial class Form1 : Form
    {
        private List<string[]> data = new List<string[]>
        {
            new string[] { "1", "데미안", "헤르만 헤세", "소설", "한 청년의 자아 발견과 성장에 대한 이야기." },
            new string[] { "2", "어린 왕자", "앙투안 드 생텍쥐페리", "동화", "어린 왕자와 그의 다양한 만남에 대한 이야기." },
            new string[] { "3", "삼국지", "나관중", "역사", "중국 후한 말기와 삼국 시대를 배경으로 한 역사 소설." },
            new string[] { "4", "해리 포터와 마법사의 돌", "J.K. 롤링", "판타지", "마법 학교 호그와트에서 펼쳐지는 해리 포터의 첫 번째 모험." },
            new string[] { "5", "나미야 잡화점의 기적", "히가시노 게이고", "추리 소설", "폐업한 잡화점에서 벌어지는 신비한 상담 사건들." }
        };

        private SortOrder sortOrder = SortOrder.Ascending;
        private int sortColumn = -1;
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수

        public Form1()
        {
            InitializeComponent();

            // list뷰 칼럼 추가
            lv_list.Columns.Add("ID", 50);
            lv_list.Columns.Add("Title", 150);
            lv_list.Columns.Add("Writer", 100);
            lv_list.Columns.Add("Genre", 100);
            lv_list.Columns.Add("Description", 300);
            lv_list.View = View.Details;

            //임시 데이터 추가
            UpdateListView();

            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //검색
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower();
            UpdateListView(searchText);
        }

        private void UpdateListView(string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                if (string.IsNullOrEmpty(searchText) || row.Any(col => col.ToLower().Contains(searchText)))
                {
                    ListViewItem item = new ListViewItem(row);
                    lv_list.Items.Add(item);
                }
            }
        }
        //
        //삽입
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

            // 새로운 ID 생성
            int newId = data.Count + 1;

            // 새로운 도서 데이터 추가
            string[] newBook = new string[] { newId.ToString(), title, writer, genre, description };
            data.Add(newBook);

            // list뷰 업데이트
            UpdateListView();

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 텍스트박스 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }

        //리스트 칼럼 정렬
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

        // ListViewItemComparer 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;

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
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                            ((ListViewItem)y).SubItems[col].Text);
                if (order == SortOrder.Descending)
                    returnVal *= -1;
                return returnVal;
            }
        }

        // 리스트뷰 항목 클릭 시 텍스트박스에 내용 출력
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lv_list.SelectedItems[0];
                selectedIndex = lv_list.Items.IndexOf(selectedItem); // 선택된 항목의 인덱스 저장
                tb_title.Text = selectedItem.SubItems[1].Text;
                tb_writer.Text = selectedItem.SubItems[2].Text;
                tb_genre.Text = selectedItem.SubItems[3].Text;
                tb_description.Text = selectedItem.SubItems[4].Text;
            }
        }

        //수정
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                // 텍스트박스의 수정된 내용을 읽어옴
                /*string title = tb_title.Text.Trim();
                string writer = tb_writer.Text.Trim();
                string genre = tb_genre.Text.Trim();
                string description = tb_description.Text.Trim();

                // 선택된 항목의 데이터 수정
                data[selectedIndex][1] = title;
                data[selectedIndex][2] = writer;
                data[selectedIndex][3] = genre;
                data[selectedIndex][4] = description;*/

                //텍스트박스 읽어와서 데이터 수정(최적화)
                data[selectedIndex][1] = tb_title.Text.Trim();
                data[selectedIndex][2] = tb_writer.Text.Trim();
                data[selectedIndex][3] = tb_genre.Text.Trim();
                data[selectedIndex][4] = tb_description.Text.Trim();

                // 리스트뷰 업데이트
                UpdateListView();

                // 선택된 항목 초기화
                selectedIndex = -1;

                // 텍스트박스 초기화
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }
        }

        //삭제
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                // 선택된 항목의 데이터 삭제
                data.RemoveAt(selectedIndex);

                // 리스트뷰 업데이트
                UpdateListView();

                // 전체 도서수 업데이트
                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                // 선택된 항목 초기화
                selectedIndex = -1;

                // 텍스트박스 초기화
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }
        }
    }
}
