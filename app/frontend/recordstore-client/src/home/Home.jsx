import { useEffect, useState } from 'react';
import {
  Center,
  Heading,
  Text,
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  Grid,
  GridItem,
  Image,
  LinkBox,
  LinkOverlay,
  CircularProgress,
  Flex,
  Box,
  Input,
  RangeSlider,
  RangeSliderTrack,
  RangeSliderFilledTrack,
  RangeSliderThumb,
  Button,
  Container,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  MenuGroup,
  MenuDivider,
  Portal,
  Select,
  HStack,
} from '@chakra-ui/react';

import { SearchIcon, AddIcon, StarIcon } from '@chakra-ui/icons';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteItem,
  AutoCompleteList,
} from '@choc-ui/chakra-autocomplete';

import { Link, Link as ReactRouterLink } from 'react-router-dom';
import productService from '../hooks/ProductService';
import genreService from '../hooks/GenreService';
import formatService from '../hooks/FormatService';

import useAuth from '../hooks/useAuth';

import usePages from '../hooks/usePages';

const Home = () => {
  const [products, setProducts] = useState([]);
  const [search, setSearch] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [title, setTitle] = useState('');
  const [artist, setArtist] = useState('');
  const [genre, setGenre] = useState('');
  const [format, setFormat] = useState('');
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(0);
  const [orderBy, setOrderBy] = useState('price');
  const [order, setOrder] = useState('asc');

  const [totalCount, setTotalCount] = useState(0);

  const [prices, setPrices] = useState({});

  const { user } = useAuth();

  const { page, nextPage, prevPage, totalPages, setTotalPages } = usePages();

  useEffect(() => {
    const fetchPrices = async () => {
      const prices = await productService.getMinMaxPrices();
      setPrices(prices);
    };

    fetchPrices();
  }, []);

  useEffect(() => {
    const fetchProducts = async () => {
      const filterParams = {};

      if (title) {
        filterParams.Title = title;
      }
      if (title !== '') {
        filterParams.Title = title;
      }
      if (artist !== '') {
        filterParams.Artist = artist;
      }
      if (genre !== '') {
        filterParams.Genre = genre;
      }
      if (format !== '') {
        filterParams.Format = format;
      }
      if (minPrice !== 0) {
        filterParams.MinPrice = minPrice;
      }
      if (maxPrice !== 0) {
        filterParams.MaxPrice = maxPrice;
      }
      filterParams.OrderBy = orderBy;
      filterParams.OrderDirection = order;
      filterParams.Page = page;

      const products = await productService.getProducts(filterParams);
      setProducts(products.results);
      setTotalPages(products.pageCount);
      setTotalCount(products.rowCount);
    };

    setIsLoading(true);
    fetchProducts();
    setIsLoading(false);
  }, [search, orderBy, order, page]);

  const [genreOptions, setGenreOptions] = useState([]);
  const [genreSearch, setGenreSearch] = useState('');
  const [isGenreLoading, setIsGenreLoading] = useState(false);

  const [formatOptions, setFormatOptions] = useState([]);
  const [formatSearch, setFormatSearch] = useState('');
  const [isFormatLoading, setIsFormatLoading] = useState(false);

  useEffect(() => {
    const fetchFormats = async () => {
      const formats = await formatService.searchFormats(formatSearch);
      console.log(formats);
      setFormatOptions(formats);
    };

    setIsFormatLoading(true);
    fetchFormats();
    setIsFormatLoading(false);
  }, [formatSearch]);

  useEffect(() => {
    const fetchGenres = async () => {
      const genres = await genreService.searchGenres(genreSearch);
      console.log(genres);
      setGenreOptions(genres);
    };

    setIsGenreLoading(true);
    fetchGenres();
    setIsGenreLoading(false);
  }, [genreSearch]);

  const handleSearch = () => {
    setSearch(!search);
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      {isLoading ? (
        <Center>
          <CircularProgress isIndeterminate size='60px' color='blue.500' />
        </Center>
      ) : (
        <Container maxW='8xl'>
          <Flex justify='space-between' align='center' mb={8}>
            <Heading size='xl' fontWeight='bold'>
              Знайден{totalCount % 10 == 1 ? 'а' : 'о'} {totalCount}{' '}
              {totalCount % 10 === 1
                ? 'позиція'
                : totalCount % 10 < 5 && totalCount % 10 !== 0
                ? 'позиції'
                : 'позицій'}
            </Heading>
            <Box>
              <Menu>
                {user &&
                  (user.role === 'admin' || user.role === 'employee') && (
                    <MenuButton
                      as={Button}
                      colorScheme='green'
                      rightIcon={<AddIcon />}
                      mr={4}
                    >
                      Додати
                    </MenuButton>
                  )}
                <Portal>
                  <MenuList>
                    <MenuGroup title='Новий'>
                      <MenuItem as={Link} to='/products/new'>
                        Продукт
                      </MenuItem>
                      <MenuItem as={Link} to='/records/new'>
                        Запис
                      </MenuItem>
                    </MenuGroup>
                  </MenuList>
                </Portal>
              </Menu>
              <Button
                colorScheme='blue'
                onClick={handleSearch}
                rightIcon={<SearchIcon />}
              >
                Пошук
              </Button>
            </Box>
          </Flex>
          <Flex>
            <Box bg='white' borderRadius='md' boxShadow='md' p={6} mr={8}>
              <Heading size='md' mb={4}>
                Фільтри
              </Heading>
              <Input
                placeholder='Назва'
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                mb={4}
              />
              <Input
                placeholder='Виконавець'
                value={artist}
                onChange={(e) => setArtist(e.target.value)}
                mb={4}
              />
              <Flex gap={4} mb={4}>
                <AutoComplete openOnFocus isLoading={isFormatLoading} mb={4}>
                  <AutoCompleteInput
                    placeholder='Формат'
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setFormat('');
                      }
                      setFormatSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {formatOptions.map((option, index) => (
                      <AutoCompleteItem
                        key={index}
                        onClick={() => setFormat(option.name)}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
                <AutoComplete openOnFocus isLoading={isGenreLoading} mb={4}>
                  <AutoCompleteInput
                    placeholder='Жанр'
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setGenre('');
                      }
                      setGenreSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {genreOptions.map((option) => (
                      <AutoCompleteItem
                        key={option.name}
                        onClick={() => setGenre(option.name)}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
              </Flex>
              <Flex justify='space-between' align='center' mb={4}>
                <Text>Ціна: {minPrice} ₴</Text>
                <Text>{maxPrice} ₴</Text>
              </Flex>
              <RangeSlider
                min={prices.minPrice}
                max={prices.maxPrice}
                step={1}
                defaultValue={[minPrice, maxPrice]}
                onChange={(values) => {
                  setMinPrice(values[0]);
                  setMaxPrice(values[1]);
                }}
                mb={4}
              >
                <RangeSliderTrack>
                  <RangeSliderFilledTrack bg='blue.500' />
                </RangeSliderTrack>
                <RangeSliderThumb boxSize={6} index={0} />
                <RangeSliderThumb boxSize={6} index={1} />
              </RangeSlider>
              <Flex justify='space-between' mb={4} gap={4}>
                <Select
                  value={orderBy}
                  onChange={(e) => setOrderBy(e.target.value)}
                  placeholder='Order By'
                >
                  <option value='title'>Title</option>
                  <option value='price'>Price</option>
                  <option value='releaseDate'>Release Date</option>
                  <option value='rating'>Rating</option>
                  <option value='reviewCount'>Review Count</option>
                </Select>
                <Select
                  value={order}
                  onChange={(e) => setOrder(e.target.value)}
                  placeholder='Order Direction'
                >
                  <option value='asc'>Ascending</option>
                  <option value='desc'>Descending</option>
                </Select>
              </Flex>
            </Box>
            <Grid
              templateColumns={[
                'repeat(1, 1fr)',
                'repeat(2, 1fr)',
                'repeat(3, 1fr)',
                'repeat(4, 1fr)',
              ]}
              gap={6}
              flex='1'
            >
              {products.length === 0 && (
                <Center>
                  <Text>Нічого не знайдено</Text>
                </Center>
              )}
              {products.map((product, index) => (
                <Box
                  bg='white'
                  borderRadius='2xl'
                  boxShadow='md'
                  overflow='hidden'
                  key={index}
                  _hover={{ boxShadow: 'xl', transform: 'translateY(-5px)' }}
                  transition={'all 0.2s ease-in-out'}
                >
                  <LinkBox
                    as='article'
                    overflow='hidden'
                    display={'flex'}
                    flexDir={'column'}
                    h='100%'
                  >
                    <Image
                      src={product.imageUrl}
                      fallbackSrc='https://via.placeholder.com/150'
                      alt='product image'
                      objectFit='cover'
                      maxH='240px'
                      w='100%'
                    />
                    <LinkOverlay
                      as={ReactRouterLink}
                      to={`/products/${product.id}`}
                      flexGrow={1}
                    >
                      <Box p={4}>
                        <Heading size='md' fontWeight='semibold' mb={2}>
                          {product.title}
                        </Heading>
                        <Text fontSize='sm' color='gray.500' mb={1}>
                          {product.format.name}
                        </Text>
                        <Text fontSize='sm' mb={2}>
                          {product.artists.map((artist, index) =>
                            index === 0 ? (
                              <span key={index}>{artist.name}</span>
                            ) : (
                              <span key={index}>, {artist.name}</span>
                            )
                          )}
                        </Text>
                        <Flex align='center' mb={2}>
                          <Flex align='center'>
                            <StarIcon color='yellow.500' />
                            <Text fontSize='sm' color='yellow.500' ml={2}>
                              {product.averageRating.toFixed(1)}
                            </Text>
                          </Flex>
                          <Text fontSize='sm' color='gray.500' ml={2}>
                            ({product.totalRatings})
                          </Text>
                        </Flex>
                      </Box>
                    </LinkOverlay>
                    <Flex
                      align='center'
                      justify='space-between'
                      bg='gray.100'
                      p={4}
                      borderTopWidth='1px'
                      borderTopColor='gray.200'
                    >
                      <Text color='blue.500' fontWeight='bold'>
                        {product.price} ₴
                      </Text>
                    </Flex>
                  </LinkBox>
                </Box>
              ))}
            </Grid>
          </Flex>
          <HStack mt={4} justify='center'>
            <Button
              isDisabled={page === 1}
              onClick={() => prevPage()}
              size='sm'
            >
              Попередня
            </Button>
            <Text mx={2}>{page}</Text>
            <Button
              isDisabled={page === totalPages}
              onClick={() => nextPage()}
              size='sm'
            >
              Наступна
            </Button>
          </HStack>
        </Container>
      )}
    </Box>
  );
};
export default Home;
